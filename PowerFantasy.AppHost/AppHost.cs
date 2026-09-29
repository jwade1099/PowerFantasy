var builder = DistributedApplication.CreateBuilder( args );

var cache = builder.AddRedis( "cache" )
    .WithLifetime( ContainerLifetime.Persistent );

var postgres = builder.AddPostgres( "postgres" )
    .WithDataVolume()
    .WithLifetime( ContainerLifetime.Persistent );

var powerFantasyDb = postgres.AddDatabase( "powerfantasydb" );

var apiService = builder.AddProject<Projects.PowerFantasy_ApiService>( "apiservice" )
    .WithHttpHealthCheck( "/health" )
    .WithReference( cache )
    .WithReference( powerFantasyDb )
    .WaitFor( powerFantasyDb );

builder.AddProject<Projects.PowerFantasy_Web>( "webfrontend" )
    .WithExternalHttpEndpoints()
    .WithHttpHealthCheck( "/health" )
    .WithReference( apiService )
    .WithReference( powerFantasyDb )
    .WaitFor( apiService )
    .WaitFor( powerFantasyDb );

builder.Build().Run();
