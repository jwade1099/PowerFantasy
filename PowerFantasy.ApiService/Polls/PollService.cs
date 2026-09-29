using Microsoft.EntityFrameworkCore;
using PowerFantasy.ApiService.Data;
using PowerFantasy.ApiService.Sleeper;

namespace PowerFantasy.ApiService.Polls;

public class PollService( PowerFantasyDbContext db, SleeperClient sleeper, LeagueSyncService sync )
{
    public async Task<WeeklyPoll> GetOrCreateCurrentPollAsync( League league, CancellationToken ct = default )
    {
        var state = await sleeper.GetNflStateAsync( ct )
            ?? throw new InvalidOperationException( "Could not determine the current NFL week from Sleeper." );

        var week = Math.Max( state.Week, 1 );

        var poll = await db.WeeklyPolls
            .FirstOrDefaultAsync( p => p.LeagueId == league.Id && p.Season == state.Season && p.Week == week, ct );

        if (poll is null)
        {
            poll = new WeeklyPoll { LeagueId = league.Id, Season = state.Season, Week = week };
            db.WeeklyPolls.Add( poll );
            await db.SaveChangesAsync( ct );
        }

        return poll;
    }

    public async Task<PollDto> BuildPollDtoAsync( League league, WeeklyPoll poll, CancellationToken ct = default )
    {
        var teams = await sync.SyncTeamsAsync( league, ct );
        var votesSubmitted = await db.PollVotes.CountAsync( v => v.WeeklyPollId == poll.Id, ct );

        return new PollDto(
            poll.Id,
            poll.Season,
            poll.Week,
            poll.CreatedAt,
            teams.Select( ToDto ).ToList(),
            votesSubmitted,
            teams.Count );
    }

    public async Task<SubmitVoteResult> SubmitVoteAsync( Guid leagueId, Guid pollId, SubmitVoteRequest request, CancellationToken ct = default )
    {
        var poll = await db.WeeklyPolls.FirstOrDefaultAsync( p => p.Id == pollId && p.LeagueId == leagueId, ct );
        if (poll is null)
        {
            return SubmitVoteResult.PollNotFound;
        }

        var teamIds = (await db.Teams.Where( t => t.LeagueId == leagueId ).Select( t => t.Id ).ToListAsync( ct )).ToHashSet();

        if (!teamIds.Contains( request.VoterTeamId ))
        {
            return SubmitVoteResult.InvalidVoter;
        }

        if (request.RankedTeamIds.Count != teamIds.Count ||
            request.RankedTeamIds.ToHashSet().Count != request.RankedTeamIds.Count ||
            !request.RankedTeamIds.All( teamIds.Contains ))
        {
            return SubmitVoteResult.InvalidBallot;
        }

        var existing = await db.PollVotes
            .FirstOrDefaultAsync( v => v.WeeklyPollId == pollId && v.VoterTeamId == request.VoterTeamId, ct );

        if (existing is null)
        {
            db.PollVotes.Add( new PollVote
            {
                WeeklyPollId = pollId,
                VoterTeamId = request.VoterTeamId,
                RankedTeamIds = request.RankedTeamIds
            } );
        }
        else
        {
            existing.RankedTeamIds = request.RankedTeamIds;
            existing.SubmittedAt = DateTimeOffset.UtcNow;
        }

        await db.SaveChangesAsync( ct );
        return SubmitVoteResult.Success;
    }

    public async Task<PollResultsDto?> GetResultsAsync( Guid leagueId, Guid pollId, CancellationToken ct = default )
    {
        var poll = await db.WeeklyPolls.FirstOrDefaultAsync( p => p.Id == pollId && p.LeagueId == leagueId, ct );
        if (poll is null)
        {
            return null;
        }

        var teams = await db.Teams.Where( t => t.LeagueId == leagueId ).ToListAsync( ct );
        var votes = await db.PollVotes.Where( v => v.WeeklyPollId == pollId ).ToListAsync( ct );

        var rankSums = teams.ToDictionary( t => t.Id, _ => (Sum: 0.0, Count: 0) );

        foreach (var vote in votes)
        {
            for (var i = 0; i < vote.RankedTeamIds.Count; i++)
            {
                var teamId = vote.RankedTeamIds[i];
                if (rankSums.TryGetValue( teamId, out var agg ))
                {
                    rankSums[teamId] = (agg.Sum + (i + 1), agg.Count + 1);
                }
            }
        }

        var rankings = teams
            .Select( t =>
            {
                var (sum, count) = rankSums[t.Id];
                var average = count > 0 ? sum / count : teams.Count + 1.0;
                return new PollResultDto( t.Id, t.TeamName, t.ManagerName, t.AvatarUrl, t.Wins, t.Losses, t.Ties, Math.Round( average, 2 ), count );
            } )
            .OrderBy( r => r.AverageRank )
            .ToList();

        return new PollResultsDto( poll.Id, poll.Season, poll.Week, votes.Count, teams.Count, rankings );
    }

    public async Task<List<PollSummaryDto>> GetHistoryAsync( Guid leagueId, CancellationToken ct = default )
    {
        var totalTeams = await db.Teams.CountAsync( t => t.LeagueId == leagueId, ct );

        return await db.WeeklyPolls
            .Where( p => p.LeagueId == leagueId )
            .OrderByDescending( p => p.Season )
            .ThenByDescending( p => p.Week )
            .Select( p => new PollSummaryDto(
                p.Id, p.Season, p.Week, p.CreatedAt,
                db.PollVotes.Count( v => v.WeeklyPollId == p.Id ),
                totalTeams ) )
            .ToListAsync( ct );
    }

    private static TeamDto ToDto( Team t ) => new( t.Id, t.TeamName, t.ManagerName, t.AvatarUrl, t.Wins, t.Losses, t.Ties );
}
