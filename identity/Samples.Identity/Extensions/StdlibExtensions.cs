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

    /// <summary>
    /// Returns a required configuration string from the configuration store.
    /// </summary>
    /// <param name="configuration">The configuration store (root or section).</param>
    /// <param name="key">The key of the required value.</param>
    /// <returns>The value of the key as a string.</returns>
    /// <exception cref="KeyNotFoundException">If the key does not exist in the configuration store.</exception>
    public static string GetRequiredString(this IConfiguration configuration, string key)
        => configuration.HasKey(key) ? configuration[key]! : throw new KeyNotFoundException($"Configuration key '{key}' not found");

    /// <summary>
    /// Determines if the provided key has a value.
    /// </summary>
    /// <param name="configuration">The configuration (root or section).</param>
    /// <param name="key">The key to look for, relative to the root of the provided configuration section.</param>
    /// <returns><c>true</c> if the key exists; false otherwise.</returns>
    public static bool HasKey(this IConfiguration configuration, string key)
        => !string.IsNullOrWhiteSpace(configuration[key]);
}
