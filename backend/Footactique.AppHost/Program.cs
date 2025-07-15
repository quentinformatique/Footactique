var builder = DistributedApplication.CreateBuilder(args);

var apiService = builder.AddProject<Projects.Footactique_ApiService>("apiservice");

builder.AddProject<Projects.Footactique_Web>("webfrontend")
    .WithExternalHttpEndpoints()
    .WithReference(apiService)
    .WaitFor(apiService);

builder.Build().Run();
