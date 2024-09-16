using System.Text.Json;

namespace Samples.Identity.Extensions;

/// <summary>
/// A set of extension methods to the BCL.
/// </summary>
internal static class StdlibExtensions
{
    /// <summary>
    /// Converts the object to a JSON string - normally for logging.
    /// </summary>
    /// <param name="obj">The object to serialize.</param>
    /// <param name="serializerOptions">The serializer options; use defaults if not specified.</param>
    /// <returns>The JSON serialization of the object.</returns>
    internal static string ToJsonString(this object obj, JsonSerializerOptions? serializerOptions = null)
    {
        serializerOptions ??= Defaults.SerializerOptions;
        return JsonSerializer.Serialize(obj, serializerOptions);
    }
}
