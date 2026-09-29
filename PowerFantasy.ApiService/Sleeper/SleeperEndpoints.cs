namespace PowerFantasy.ApiService.Sleeper;

public static class SleeperEndpoints
{
    public static void MapSleeperEndpoints( this WebApplication app )
    {
        app.MapGet( "/api/sleeper/usage", async ( SleeperClient sleeper, CancellationToken ct ) =>
            Results.Ok( await sleeper.GetRecentUsageAsync( ct: ct ) ) );

        app.MapGet( "/api/sleeper/league/{sleeperLeagueId}", async ( string sleeperLeagueId, SleeperClient sleeper, CancellationToken ct ) =>
        {
            var league = await sleeper.GetLeagueAsync( sleeperLeagueId, ct );
            return league is null ? Results.NotFound() : Results.Ok( new { league.Name, league.Season } );
        } );
    }
}
