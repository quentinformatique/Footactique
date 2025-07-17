var builder = DistributedApplication.CreateBuilder(args);

// --- Database ---
var postgresServer = builder.AddPostgres("postgreSQLServer").WithPgAdmin();
var appDatabase = postgresServer.AddDatabase("footactique");

// --- API Service ---
var api = builder.AddProject<Projects.Footactique_Api>("web")
    .WithReference(appDatabase)
    .WaitFor(postgresServer)
    .WaitFor(appDatabase);

builder.Build().Run();