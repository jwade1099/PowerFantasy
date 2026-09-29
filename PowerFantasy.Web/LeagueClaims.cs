using System.Security.Claims;

namespace PowerFantasy.Web;

public static class LeagueClaims
{
    public static Guid? GetLeagueId( ClaimsPrincipal user ) =>
        Guid.TryParse( user.FindFirst( "league_id" )?.Value, out var id ) ? id : null;
}
