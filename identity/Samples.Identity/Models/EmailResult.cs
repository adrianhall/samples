namespace Samples.Identity.Models;

/// <summary>
/// The response from an email send operation.
/// </summary>
public class EmailResult
{
    /// <summary>
    /// Was it successful?
    /// </summary>
    public bool Succeeded { get; set; }

    /// <summary>
    /// The (API-specific) result code.
    /// </summary>
    public int ResultCode { get; set; }

    /// <summary>
    /// Any messages that were generated.
    /// </summary>
    public string[] Messages { get; set; } = [];

    /// <summary>
    /// Logs the result of the email send operation.
    /// </summary>
    /// <param name="logger"></param>
    public void LogResult(ILogger logger)
    {
        if (Succeeded)
        {
            logger.LogInformation("Email send succeeded");
        }
        else
        {
            logger.LogError("Email send failed: Result code = {resultCode}", ResultCode);
            foreach (string message in Messages)
            {
                logger.LogError("Message: {message}", message);
            }
        }

    }
}
