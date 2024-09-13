using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace Samples.Identity.Extensions;

internal static class ModelStateExtensions
{
    internal static IEnumerable<string> GetErrorMessages(this ModelStateDictionary modelState)
        => modelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage);

    internal static string GetErrorMessage(this ModelStateDictionary modelState)
        => modelState.GetErrorMessages().FirstOrDefault() ?? string.Empty;

}
