using PowerFantasy.ApiService.Leagues;
using PowerFantasy.ApiService.Sleeper;

namespace PowerFantasy.ApiService.Polls;

public static class PollEndpoints
{
    public static void MapPollEndpoints( this WebApplication app )
    {
        var group = app.MapGroup( "/api/leagues/{leagueId:guid}" );

        group.MapGet( "/teams", async ( Guid leagueId, LeagueService leagueService, LeagueSyncService sync, CancellationToken ct ) =>
        {
            var league = await leagueService.GetEntityAsync( leagueId, ct );
            if (league is null) return Results.NotFound( "League not found." );

            var teams = await sync.SyncTeamsAsync( league, ct );
            return Results.Ok( teams.Select( t => new TeamDto( t.Id, t.TeamName, t.ManagerName, t.AvatarUrl, t.Wins, t.Losses, t.Ties ) ) );
        } );

        group.MapGet( "/polls/current", async ( Guid leagueId, LeagueService leagueService, PollService polls, CancellationToken ct ) =>
        {
            var league = await leagueService.GetEntityAsync( leagueId, ct );
            if (league is null) return Results.NotFound( "League not found." );

            var poll = await polls.GetOrCreateCurrentPollAsync( league, ct );
            var dto = await polls.BuildPollDtoAsync( league, poll, ct );
            return Results.Ok( dto );
        } );

        group.MapPost( "/polls/{pollId:guid}/votes", async ( Guid leagueId, Guid pollId, SubmitVoteRequest request, PollService polls, CancellationToken ct ) =>
        {
            var result = await polls.SubmitVoteAsync( leagueId, pollId, request, ct );
            return result switch
            {
                SubmitVoteResult.Success => Results.NoContent(),
                SubmitVoteResult.PollNotFound => Results.NotFound( "Poll not found." ),
                SubmitVoteResult.InvalidVoter => Results.BadRequest( "Voter is not a recognized team." ),
                SubmitVoteResult.InvalidBallot => Results.BadRequest( "Ballot must rank every team exactly once." ),
                _ => Results.Problem()
            };
        } );

        group.MapGet( "/polls/{pollId:guid}/results", async ( Guid leagueId, Guid pollId, PollService polls, CancellationToken ct ) =>
        {
            var results = await polls.GetResultsAsync( leagueId, pollId, ct );
            return results is null ? Results.NotFound() : Results.Ok( results );
        } );

        group.MapGet( "/polls/history", async ( Guid leagueId, PollService polls, CancellationToken ct ) =>
        {
            var history = await polls.GetHistoryAsync( leagueId, ct );
            return Results.Ok( history );
        } );
    }
}
