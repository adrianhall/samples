using Aspire.Hosting.Azure;

var builder = DistributedApplication.CreateBuilder(args);

IResourceBuilder<AzureAppConfigurationResource>? appConfig = null;

if (builder.ExecutionContext.IsPublishMode)
{
    appConfig = builder.AddAzureAppConfiguration("appconfig");
    builder.AddAzureKeyVault("keyvault");
}

var svc = builder.AddProject<Projects.SampleWebApp>("samplewebapp")
    .WithExternalHttpEndpoints();

if (appConfig is not null)
{
    svc.WithReference(appConfig);
}

builder.Build().Run();
