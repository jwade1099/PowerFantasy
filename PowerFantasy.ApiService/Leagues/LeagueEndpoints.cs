namespace PowerFantasy.ApiService.Leagues;

public static class LeagueEndpoints
{
    public static void MapLeagueEndpoints( this WebApplication app )
    {
        app.MapPost( "/api/leagues", async ( CreateLeagueRequest request, LeagueService leagues, CancellationToken ct ) =>
        {
            var (result, league) = await leagues.CreateLeagueAsync( request, ct );
            return result switch
            {
                CreateLeagueResult.Success => Results.Ok( league ),
                CreateLeagueResult.SleeperLeagueNotFound => Results.NotFound( "That Sleeper league ID couldn't be found." ),
                CreateLeagueResult.AlreadyClaimed => Results.Conflict( "That Sleeper league has already been claimed by a commissioner." ),
                _ => Results.Problem()
            };
        } );

        app.MapGet( "/api/leagues/mine/{commissionerUserId}", async ( string commissionerUserId, LeagueService leagues, CancellationToken ct ) =>
        {
            var league = await leagues.GetByCommissionerAsync( commissionerUserId, ct );
            return league is null ? Results.NotFound() : Results.Ok( league );
        } );

        app.MapGet( "/api/leagues/by-slug/{slug}", async ( string slug, LeagueService leagues, CancellationToken ct ) =>
        {
            var league = await leagues.GetBySlugAsync( slug, ct );
            return league is null ? Results.NotFound() : Results.Ok( league );
        } );
    }
}
