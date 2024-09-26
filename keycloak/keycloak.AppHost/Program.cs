var builder = DistributedApplication.CreateBuilder(args);

var keycloak = builder
    .AddKeycloakContainer("keycloak")
    .WithDataVolume()
    .WithImport("./KeycloakConfiguration/Test-realm.json")
    .WithImport("./KeycloakConfiguration/Test-users-0.json");

var realm = keycloak.AddRealm("Test");

var apiService = builder
    .AddProject<Projects.keycloak_ApiService>("apiservice")
    .WithReference(realm)
    .WithEnvironment("Keycloak__ClientId", "workspaces-client")
    .WithEnvironment("Keycloak__ClientSecret", "ze4SQDpbyBlB72kdTCTv8ecSWsJHf2Js");

builder.AddProject<Projects.keycloak_Web>("webfrontend")
    .WithExternalHttpEndpoints()
    .WithReference(apiService)
    .WithReference(realm)
    .WithEnvironment("Keycloak__ClientId", "workspaces-client")
    .WithEnvironment("Keycloak__ClientSecret", "ze4SQDpbyBlB72kdTCTv8ecSWsJHf2Js");

builder.Build().Run();
