using System.Security.Claims;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;

namespace PowerFantasy.Web.Data;

/// Embeds the commissioner's league id/slug as claims at sign-in, so authorized pages
/// never need an extra round-trip to find out which league they're scoped to.
public class ApplicationClaimsPrincipalFactory( UserManager<ApplicationUser> userManager, IOptions<IdentityOptions> options, PowerFantasyApiClient api )
    : UserClaimsPrincipalFactory<ApplicationUser>( userManager, options )
{
    protected override async Task<ClaimsIdentity> GenerateClaimsAsync( ApplicationUser user )
    {
        var identity = await base.GenerateClaimsAsync( user );

        var league = await api.GetMyLeagueAsync( user.Id );
        if (league is not null)
        {
            identity.AddClaim( new Claim( "league_id", league.Id.ToString() ) );
            identity.AddClaim( new Claim( "league_slug", league.PublicSlug ) );
            identity.AddClaim( new Claim( "league_name", league.Name ) );
        }

        return identity;
    }
}
