using PowerFantasy.ApiService.Leagues;

namespace PowerFantasy.ApiService.Dashboard;

public static class DashboardEndpoints
{
    public static void MapDashboardEndpoints( this WebApplication app )
    {
        app.MapGet( "/api/leagues/{leagueId:guid}/dashboard", async ( Guid leagueId, LeagueService leagueService, DashboardService dashboard, CancellationToken ct ) =>
        {
            var league = await leagueService.GetEntityAsync( leagueId, ct );
            if (league is null) return Results.NotFound( "League not found." );

            return Results.Ok( await dashboard.GetDashboardAsync( league, ct ) );
        } );
    }
}
