using Microsoft.Extensions.Hosting;

var builder = DistributedApplication.CreateBuilder(args);

// =================================================================
// Database Services
// =================================================================

var databaseService = builder.AddPostgres("postgres");
if (builder.Environment.IsDevelopment())
{
    databaseService.WithPgWeb();
}

// =================================================================
// Identity Sample
// =================================================================

var identitydb = databaseService.AddDatabase("identitydb");

builder.AddProject<Projects.Samples_Identity>("identityservice")
    .WithReference(identitydb)
    .WithExternalHttpEndpoints();

// =================================================================
// Run the orchestration
// =================================================================

builder.Build().Run();
