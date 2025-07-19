using Aspire.Hosting;

IDistributedApplicationBuilder builder = DistributedApplication.CreateBuilder(args);

// Add the API service
IResourceBuilder<ProjectResource> api = builder.AddProject<Projects.Footactique_Api>("web");

builder.Build().Run();