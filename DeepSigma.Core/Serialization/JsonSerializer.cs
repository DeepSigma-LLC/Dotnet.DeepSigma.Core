using Newtonsoft.Json;

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
