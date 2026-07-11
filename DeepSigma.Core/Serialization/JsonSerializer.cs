using Newtonsoft.Json;
using System.Text.Json;

namespace DeepSigma.Core.Serialization;

/// <summary>
/// Utility class for serialization and deserialization of objects to and from JSON strings.
/// </summary>
public static class JsonSerializer
{
    /// <summary>
    /// Serializes an object to a JSON string.
    /// </summary>
    /// <param name="obj"></param>
    /// <returns></returns>
    public static string GetSerializedString(object obj)
    {
        return JsonConvert.SerializeObject(obj);
    }

    /// <summary>
    /// Serializes an object to a JSON string using System.Text.Json.
    /// </summary>
    /// <param name="obj"></param>
    /// <param name="options"></param>
    /// <returns></returns>
    public static string GetSerializedString(object obj, JsonSerializerOptions? options = null)
    {
        options ??= new JsonSerializerOptions();
        return System.Text.Json.JsonSerializer.Serialize(obj, options);
    }

    /// <summary>
    /// Deserializes a JSON string to an object of type T.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="JSONString"></param>
    /// <returns></returns>
    public static T? GetDeserializedObject<T>(string JSONString)
    {
        return JsonConvert.DeserializeObject<T>(JSONString);
    }
}
