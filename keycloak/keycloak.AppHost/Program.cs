var builder = DistributedApplication.CreateBuilder(args);

var username = builder.AddParameter("username");
var password = builder.AddParameter("password");

var dbserver = builder
    .AddPostgres("postgres", username, password)
    .WithEnvironment("POSTGRES_DB", "identity")
    .WithPgAdmin();

var identitydb = dbserver.AddDatabase("identity");
        
var keycloak = builder
    .AddKeycloakContainer("keycloak")
    .WithDockerfile("./KeycloakConfiguration", "Dockerfile")
    .WithHttpsEndpoint(port: 8443, targetPort: 8443, name: "https")
    .WithDataVolume()
    .WithEnvironment("KC_DB_USERNAME", username)
    .WithEnvironment("KC_DB_PASSWORD", password)
    .WithEnvironment("KC_DB_URL", JdbcExpression.Create(identitydb))
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
