namespace ClientApp;

internal static class Constants
{
    /// <summary>
    /// The base URI for the Datasync service.
    /// </summary>
    public static string ServiceUri = "https://localhost:7181";

    /// <summary>
    /// The application (client) ID for the native app within Microsoft Entra ID
    /// </summary>
    public static string ApplicationId = "<your-client-application-id>";

    /// <summary>
    /// The list of scopes to request
    /// </summary>
    public static string[] Scopes = ["api://<your-server-application-id>/access_as_user"];
}
