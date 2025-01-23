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
    public static string ApplicationId = "d29fecae-8fc2-4fc0-96ed-99a5191f2cfb";

    /// <summary>
    /// The list of scopes to request
    /// </summary>
    public static string[] Scopes = ["api://d4264e8a-722b-4a7e-855c-9b5f136dda62/access_as_user"];
}
