var builder = DistributedApplication.CreateBuilder( args );

var cache = builder.AddRedis( "cache" )
    .WithLifetime( ContainerLifetime.Persistent );

// Local dev runs SQL Server in a container. When publishing, the database is an external
// Azure SQL database, so its connection string is supplied as a deployment parameter instead.
IResourceBuilder<IResourceWithConnectionString> powerFantasyDb = builder.ExecutionContext.IsPublishMode
    ? builder.AddConnectionString( "powerfantasydb" )
    : builder.AddSqlServer( "sql" )
        .WithDataVolume()
        .WithLifetime( ContainerLifetime.Persistent )
        .AddDatabase( "powerfantasydb" );

// Shared secret so only the Web app can call the API. Generated per run locally;
// a required secret parameter when publishing.
var internalApiKey = builder.AddParameter(
    "internal-api-key",
    () => Convert.ToHexString( System.Security.Cryptography.RandomNumberGenerator.GetBytes( 32 ) ),
    publishValueAsDefault: false,
    secret: true );

var apiService = builder.AddProject<Projects.PowerFantasy_ApiService>( "apiservice" )
    .WithHttpHealthCheck( "/health" )
    .WithReference( cache )
    .WithReference( powerFantasyDb )
    .WithEnvironment( "InternalApi__Key", internalApiKey )
    .WaitFor( powerFantasyDb );

builder.AddProject<Projects.PowerFantasy_Web>( "webfrontend" )
    .WithExternalHttpEndpoints()
    .WithHttpHealthCheck( "/health" )
    .WithReference( apiService )
    .WithReference( powerFantasyDb )
    .WithEnvironment( "InternalApi__Key", internalApiKey )
    .WaitFor( apiService )
    .WaitFor( powerFantasyDb );

builder.Build().Run();
