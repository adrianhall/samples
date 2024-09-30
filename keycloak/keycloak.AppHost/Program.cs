var builder = DistributedApplication.CreateBuilder(args);

var keycloak = builder
    .AddKeycloakContainer("keycloak")
    .WithDockerfile("./KeycloakConfiguration", "Dockerfile")
    .WithHttpsEndpoint(port: 8443, targetPort: 8443, name: "https")
    .WithDataVolume()
    .WithImport("./KeycloakConfiguration/Test-realm.json");

if (builder.ExecutionContext.IsRunMode)
{
    keycloak.WithImport("./KeycloakConfiguration/Test-users-0.json");
}

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
