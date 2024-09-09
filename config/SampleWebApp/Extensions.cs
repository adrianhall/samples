using Azure.Core;
using Azure.Identity;
using System.Diagnostics;

namespace SampleWebApp;

public static class Extensions
{
    public static void AddAzureAppConfiguration(this IHostApplicationBuilder builder, string connectionStringName = "appconfig")
    {
        string? connectionString = builder.Configuration.GetConnectionString(connectionStringName);
        if (string.IsNullOrEmpty(connectionString))
        {
            Debug.WriteLine($"No connection string for '{connectionStringName}' - skipping app config setup");
            return;
        }
        Debug.WriteLine($"Connection string for app config: {connectionString}");

        string? clientId = builder.Configuration["AZURE_CLIENT_ID"];
        TokenCredential credential = string.IsNullOrEmpty(clientId)
            ? new DefaultAzureCredential(includeInteractiveCredentials: builder.Environment.IsDevelopment())
            : new ManagedIdentityCredential(clientId);

        Debug.WriteLine("Connecting to App Configuration...");
        builder.Configuration.AddAzureAppConfiguration(options =>
        {
            options.Connect(new Uri(connectionString), credential);
            options.ConfigureKeyVault(kv => kv.SetCredential(credential));
        });
        Debug.WriteLine("App Configuration setup complete");
    }
}
