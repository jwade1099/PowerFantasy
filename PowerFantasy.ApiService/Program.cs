using Microsoft.EntityFrameworkCore;
using PowerFantasy.ApiService.Dashboard;
using PowerFantasy.ApiService.Data;
using PowerFantasy.ApiService.Leagues;
using PowerFantasy.ApiService.Polls;
using PowerFantasy.ApiService.Sleeper;

var builder = WebApplication.CreateBuilder( args );

// Add service defaults & Aspire client integrations.
builder.AddServiceDefaults();

// Add services to the container.
builder.Services.AddProblemDetails();

// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

builder.AddRedisDistributedCache( "cache" );
builder.AddNpgsqlDbContext<PowerFantasyDbContext>( "powerfantasydb" );

builder.Services.AddHttpClient<SleeperClient>( c =>
    c.BaseAddress = new Uri( "https://api.sleeper.app/v1/" ) );

builder.Services.AddScoped<LeagueSyncService>();
builder.Services.AddScoped<LeagueService>();
builder.Services.AddScoped<PollService>();
builder.Services.AddScoped<DashboardService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
app.UseExceptionHandler();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<PowerFantasyDbContext>();
    await db.Database.MigrateAsync();
}

app.MapLeagueEndpoints();
app.MapPollEndpoints();
app.MapDashboardEndpoints();
app.MapSleeperEndpoints();

app.MapDefaultEndpoints();

app.Run();
