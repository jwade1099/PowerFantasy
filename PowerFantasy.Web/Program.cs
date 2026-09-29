using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using PowerFantasy.Web;
using PowerFantasy.Web.Components;
using PowerFantasy.Web.Data;

var builder = WebApplication.CreateBuilder( args );

// Add service defaults & Aspire client integrations.
builder.AddServiceDefaults();

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

builder.Services.AddOutputCache();

builder.Services.AddHttpClient<PowerFantasyApiClient>( client =>
    {
        // This URL uses "https+http://" to indicate HTTPS is preferred over HTTP.
        // Learn more about service discovery scheme resolution at https://aka.ms/dotnet/sdschemes.
        client.BaseAddress = new( "https+http://apiservice" );
    } );

builder.Services.AddDbContext<ApplicationIdentityDbContext>( options =>
    options.UseNpgsql(
        builder.Configuration.GetConnectionString( "powerfantasydb" ),
        npgsql => npgsql.MigrationsHistoryTable( "__EFMigrationsHistoryIdentity" ) ) );

builder.Services.AddCascadingAuthenticationState();
builder.Services.AddAuthorization();
builder.Services.AddAuthentication( IdentityConstants.ApplicationScheme )
    .AddIdentityCookies();
builder.Services.AddIdentityCore<ApplicationUser>( options => options.SignIn.RequireConfirmedAccount = false )
    .AddEntityFrameworkStores<ApplicationIdentityDbContext>()
    .AddSignInManager()
    .AddClaimsPrincipalFactory<ApplicationClaimsPrincipalFactory>();

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler( "/Error", createScopeForErrors: true );
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.UseAntiforgery();

app.UseOutputCache();

app.MapStaticAssets();

app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.MapPost( "/account/logout", async ( SignInManager<ApplicationUser> signInManager ) =>
{
    await signInManager.SignOutAsync();
    return Results.LocalRedirect( "/" );
} );

using (var scope = app.Services.CreateScope())
{
    var identityDb = scope.ServiceProvider.GetRequiredService<ApplicationIdentityDbContext>();
    await identityDb.Database.MigrateAsync();
}

app.MapDefaultEndpoints();

app.Run();
