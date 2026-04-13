using DeepSigma.Core;
using DeepSigma.Core.Utilities;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;

namespace DeepSigma.Core.Serialization;

/// <summary>
/// Provides methods for deterministic serialization of objects to byte arrays and hashes.
/// Note: This implementation ensures that the JSON serialization is consistent by sorting properties alphabetically.
/// </summary>
public static class DeterministicSerializer
{
    /// <summary>
    /// Converts an object to a deterministic byte array representation.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    public static byte[] ToDeterministicBytes<T>(T obj)
    {
        // Serialize to JSON
        string json = System.Text.Json.JsonSerializer.Serialize(obj, Options);

        // Normalize property order alphabetically
        using JsonDocument? doc = JsonDocument.Parse(json);
        string canonicalJson = System.Text.Json.JsonSerializer.Serialize<object?>(SortProperties(doc.RootElement));

        return Encoding.UTF8.GetBytes(canonicalJson);
    }

    /// <summary>
    /// Converts an object to a hash using the specified hash algorithm.
    /// </summary>
    /// <param name="hashAlgorithmName"></param>
    /// <param name="obj"></param>
    /// <returns></returns>
    public static byte[] ToDeterministicHash<T>(T obj, HashAlgorithmName hashAlgorithmName)
    {
        return HashUtilities.ComputeHash(ToDeterministicBytes(obj), hashAlgorithmName);
    }

    private static readonly JsonSerializerOptions Options = new()
    {
        WriteIndented = false,
        DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.Never,
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,

        // Critical for determinism:
        Encoder = System.Text.Encodings.Web.JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
    };

    /// <summary>
    /// Recursively sorts the properties of a JsonElement alphabetically.
    /// </summary>
    /// <param name="element"></param>
    /// <returns></returns>
    private static object? SortProperties(JsonElement element)
    {
        return element.ValueKind switch
        {
            JsonValueKind.Object => element.EnumerateObject()
                .OrderBy(p => p.Name)
                .ToDictionary<JsonProperty, string, object?>(p => p.Name, p => SortProperties(p.Value)),
            JsonValueKind.Array => element.EnumerateArray()
                .Select<JsonElement, object?>(SortProperties).ToList(),
            _ => element.GetRawText() switch
            {
                var t when bool.TryParse(t, out var b) => b,
                var t when double.TryParse(t, out var d) => d,
                var t when t == "null" => null,
                var t => System.Text.Json.JsonSerializer.Deserialize<object>(t)
            }
        };
    }
}
