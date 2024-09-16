using System.Text.Json;

namespace Samples.Identity;

/// <summary>
/// A set of defaults for various components.
/// </summary>
internal static class Defaults
{
    internal static Lazy<JsonSerializerOptions> _serializerOptions = new(() => GetJsonSerializerOptions());

    /// <summary>
    /// The default JSON serializer options.
    /// </summary>
    internal static JsonSerializerOptions SerializerOptions
    {
        get => _serializerOptions.Value;
    }

    /// <summary>
    /// Method for creating the default <see cref="JsonSerializerOptions"/> for
    /// the application.
    /// </summary>
    internal static JsonSerializerOptions GetJsonSerializerOptions()
    {
        JsonSerializerOptions options = new(JsonSerializerDefaults.General)
        {
            ReadCommentHandling = JsonCommentHandling.Skip,
            WriteIndented = false
        };

        return options;
    }
}