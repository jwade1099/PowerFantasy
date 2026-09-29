using Microsoft.EntityFrameworkCore;
using PowerFantasy.ApiService.Data;

namespace PowerFantasy.ApiService.Sleeper;

/// Pulls roster + user info from Sleeper and upserts it into the local Team table,
/// so team names and managers always mirror what's set up in Sleeper.
public class LeagueSyncService( PowerFantasyDbContext db, SleeperClient sleeper )
{
    public async Task<List<Team>> SyncTeamsAsync( League league, CancellationToken ct = default )
    {
        var rosters = await sleeper.GetRostersAsync( league.SleeperLeagueId, ct );
        var users = await sleeper.GetUsersAsync( league.SleeperLeagueId, ct );
        var usersById = users.ToDictionary( u => u.UserId );

        var existingTeams = await db.Teams
            .Where( t => t.LeagueId == league.Id )
            .ToDictionaryAsync( t => t.SleeperRosterId, ct );

        foreach (var roster in rosters)
        {
            if (roster.OwnerId is null || !usersById.TryGetValue( roster.OwnerId, out var user ))
            {
                continue;
            }

            var teamName = user.Metadata?.TeamName ?? user.DisplayName;
            var avatarUrl = user.Avatar is null ? null : $"https://sleepercdn.com/avatars/{user.Avatar}";
            var settings = roster.Settings;
            var pointsFor = (settings?.Fpts ?? 0) + (settings?.FptsDecimal ?? 0) / 100.0;
            var pointsAgainst = (settings?.FptsAgainst ?? 0) + (settings?.FptsAgainstDecimal ?? 0) / 100.0;

            if (!existingTeams.TryGetValue( roster.RosterId, out var team ))
            {
                team = new Team
                {
                    LeagueId = league.Id,
                    SleeperRosterId = roster.RosterId,
                    SleeperOwnerId = user.UserId,
                    TeamName = teamName,
                    ManagerName = user.DisplayName
                };
                db.Teams.Add( team );
            }

            team.TeamName = teamName;
            team.ManagerName = user.DisplayName;
            team.AvatarUrl = avatarUrl;
            team.SleeperOwnerId = user.UserId;
            team.Wins = settings?.Wins ?? 0;
            team.Losses = settings?.Losses ?? 0;
            team.Ties = settings?.Ties ?? 0;
            team.PointsFor = pointsFor;
            team.PointsAgainst = pointsAgainst;
        }

        await db.SaveChangesAsync( ct );

        return await db.Teams
            .Where( t => t.LeagueId == league.Id )
            .OrderBy( t => t.TeamName )
            .ToListAsync( ct );
    }
}
