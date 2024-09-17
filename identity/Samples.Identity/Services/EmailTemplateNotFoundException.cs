namespace Samples.Identity.Services;

internal class EmailTemplateNotFoundException : ApplicationException
{
    public EmailTemplateNotFoundException()
    {
    }

    public EmailTemplateNotFoundException(string? message) : base(message)
    {
    }

    public EmailTemplateNotFoundException(string? message, Exception? innerException) : base(message, innerException)
    {
    }

    public required IList<string> SearchedLocations { get; init; }
}
