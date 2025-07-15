var builder = DistributedApplication.CreateBuilder(args);

// Ensure the 'AddPostgres' extension method is available and correctly implemented.  
var postgresServer = builder.AddPostgres("postgreSQLServer").WithPgAdmin();

var exampleDatabase = postgresServer.AddDatabase("exampleDB");

builder.AddProject<Projects.Footactique_Api>("web")
   .WithReference(exampleDatabase);

builder.Build().Run();