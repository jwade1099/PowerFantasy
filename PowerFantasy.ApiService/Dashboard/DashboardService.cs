using PowerFantasy.ApiService.Data;
using PowerFantasy.ApiService.Sleeper;

namespace PowerFantasy.ApiService.Dashboard;

public class DashboardService( SleeperClient sleeper, LeagueSyncService sync )
{
    public async Task<DashboardDto> GetDashboardAsync( League league, CancellationToken ct = default )
    {
        var teams = await sync.SyncTeamsAsync( league, ct );
        var teamsByRosterId = teams.ToDictionary( t => t.SleeperRosterId );

        var state = await sleeper.GetNflStateAsync( ct )
            ?? throw new InvalidOperationException( "Could not determine the current NFL week from Sleeper." );
        var week = Math.Max( state.Week, 1 );

        var standings = teams
            .OrderByDescending( t => t.Wins )
            .ThenByDescending( t => t.PointsFor )
            .Select( ToStandingDto )
            .ToList();

        var sleeperMatchups = await sleeper.GetMatchupsAsync( league.SleeperLeagueId, week, ct );

        var matchups = sleeperMatchups
            .Where( m => m.MatchupId is not null && teamsByRosterId.ContainsKey( m.RosterId ) )
            .GroupBy( m => m.MatchupId!.Value )
            .Select( g => new MatchupDto(
                g.Key,
                g.Select( m =>
                {
                    var team = teamsByRosterId[m.RosterId];
                    return new MatchupTeamDto( team.Id, team.TeamName, team.AvatarUrl, m.Points ?? 0 );
                } ).ToList() ) )
            .OrderBy( m => m.MatchupId )
            .ToList();

        return new DashboardDto( state.Season, week, standings, matchups );
    }

    private static StandingDto ToStandingDto( Team t ) =>
        new( t.Id, t.TeamName, t.ManagerName, t.AvatarUrl, t.Wins, t.Losses, t.Ties, t.PointsFor, t.PointsAgainst );
}
