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

        var internalKey = builder.Configuration["InternalApi:Key"];
        if (!string.IsNullOrEmpty( internalKey ))
        {
            client.DefaultRequestHeaders.Add( "X-Internal-Key", internalKey );
        }
    } );

builder.Services.AddDbContext<ApplicationIdentityDbContext>( options =>
    options.UseSqlServer(
        builder.Configuration.GetConnectionString( "powerfantasydb" ),
        sql => sql
            .MigrationsHistoryTable( "__EFMigrationsHistoryIdentity" )
            .EnableRetryOnFailure() ) );

builder.Services.AddCascadingAuthenticationState();
builder.Services.AddAuthorization();
builder.Services.AddAuthentication( IdentityConstants.ApplicationScheme )
    .AddIdentityCookies();
builder.Services.ConfigureApplicationCookie( options =>
{
    options.LoginPath = "/commissioner/login";

    // TLS is terminated in front of the container in Azure, so don't rely on the request scheme.
    if (!builder.Environment.IsDevelopment())
    {
        options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
    }
} );
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
