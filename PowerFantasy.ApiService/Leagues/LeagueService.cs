using System.Security.Cryptography;
using Microsoft.EntityFrameworkCore;
using PowerFantasy.ApiService.Data;
using PowerFantasy.ApiService.Sleeper;

namespace PowerFantasy.ApiService.Leagues;

public class LeagueService( PowerFantasyDbContext db, SleeperClient sleeper )
{
    public async Task<(CreateLeagueResult Result, LeagueDto? League)> CreateLeagueAsync( CreateLeagueRequest request, CancellationToken ct = default )
    {
        var alreadyClaimed = await db.Leagues.AnyAsync( l => l.SleeperLeagueId == request.SleeperLeagueId, ct );
        if (alreadyClaimed)
        {
            return (CreateLeagueResult.AlreadyClaimed, null);
        }

        var sleeperLeague = await sleeper.GetLeagueAsync( request.SleeperLeagueId, ct );
        if (sleeperLeague is null)
        {
            return (CreateLeagueResult.SleeperLeagueNotFound, null);
        }

        var league = new League
        {
            SleeperLeagueId = request.SleeperLeagueId,
            Name = sleeperLeague.Name,
            CommissionerUserId = request.CommissionerUserId,
            PublicSlug = await GenerateUniqueSlugAsync( ct )
        };

        db.Leagues.Add( league );
        await db.SaveChangesAsync( ct );

        return (CreateLeagueResult.Success, ToDto( league ));
    }

    public async Task<LeagueDto?> GetByCommissionerAsync( string commissionerUserId, CancellationToken ct = default )
    {
        var league = await db.Leagues.FirstOrDefaultAsync( l => l.CommissionerUserId == commissionerUserId, ct );
        return league is null ? null : ToDto( league );
    }

    public async Task<LeagueDto?> GetBySlugAsync( string slug, CancellationToken ct = default )
    {
        var league = await db.Leagues.FirstOrDefaultAsync( l => l.PublicSlug == slug, ct );
        return league is null ? null : ToDto( league );
    }

    public Task<League?> GetEntityAsync( Guid leagueId, CancellationToken ct = default ) =>
        db.Leagues.FirstOrDefaultAsync( l => l.Id == leagueId, ct );

    private async Task<string> GenerateUniqueSlugAsync( CancellationToken ct )
    {
        for (var attempt = 0; attempt < 5; attempt++)
        {
            var slug = Convert.ToHexString( RandomNumberGenerator.GetBytes( 5 ) ).ToLowerInvariant();
            if (!await db.Leagues.AnyAsync( l => l.PublicSlug == slug, ct ))
            {
                return slug;
            }
        }

        throw new InvalidOperationException( "Could not generate a unique league slug." );
    }

    private static LeagueDto ToDto( League league ) => new( league.Id, league.Name, league.PublicSlug );
}
