var builder = DistributedApplication.CreateBuilder(args);

var apiService = builder.AddProject<Projects.keycloak_ApiService>("apiservice");

builder.AddProject<Projects.keycloak_Web>("webfrontend")
    .WithExternalHttpEndpoints()
    .WithReference(apiService);

builder.Build().Run();
