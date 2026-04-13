
using DeepSigma.Core.Serialization;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;

namespace DeepSigma.Core.Extensions;

/// <summary>
/// Extensions for object functionality extension.
/// </summary>
public static class ObjectExtension
{
    /// <summary>
    /// Convert an object to JSON string.
    /// </summary>
    /// <param name="value"></param>
    public static string ToJSON<T>(this T value) where T : class
    {
        ArgumentNullException.ThrowIfNull(value);
        return JsonSerializer.GetSerializedString(value);
    }

    /// <summary>
    /// Convert a JSON string to an object of type T.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="json"></param>
    /// <returns></returns>
    public static T? FromJson<T>(this string json) where T : class, new()
    {
        if(!json.IsValidJson()) return null;
        return JsonSerializer.GetDeserializedObject<T>(json);
    }

    /// <summary>
    /// Converts an object to a deterministic byte array representation.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="value"></param>
    /// <returns></returns>
    public static byte[] ToDeterministicBytes<T>(this T value) where T : class  => DeterministicSerializer.ToDeterministicBytes(value);

    /// <summary>
    /// Converts an object to a hash using the specified hash algorithm.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="value"></param>
    /// <param name="hash_algorithm_name"></param>
    /// <returns></returns>
    public static byte[] ToDeterministicHash<T>(this T value, HashAlgorithmName hash_algorithm_name) where T : class => DeterministicSerializer.ToDeterministicHash(value, hash_algorithm_name);

    /// <summary>
    /// Convert a single object to an enumerable containing that object.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="value"></param>
    /// <returns></returns>
    public static IEnumerable<T> ToEnumerable<T>(this T value)
    {
        yield return value;
    }

    /// <summary>
    /// Retrieves the value of a public property with the specified name from the given object instance.
    /// </summary>
    /// <remarks>If the property does not exist or is not public, the method returns null. This method uses
    /// reflection and may have performance implications when called frequently. The property name comparison is
    /// case-sensitive.</remarks>
    /// <param name="obj">The object instance from which to retrieve the property value. Cannot be null.</param>
    /// <param name="propertyName">The name of the public property to retrieve. The comparison is case-sensitive.</param>
    /// <returns>The value of the specified property if found; otherwise, null.</returns>
    public static object? GetPropertyValue(this object obj, string propertyName)
    {
        return obj.GetType().GetProperty(propertyName)?.GetValue(obj);
    }

    /// <summary>
    /// Sets the value of a public property on the specified object by property name.
    /// </summary>
    /// <remarks>
    /// If the specified property does not exist or is not writable, no action is taken and no
    /// exception is thrown. This method uses reflection and may have performance implications when called
    /// frequently.
    /// </remarks>
    /// <param name="obj">The object whose property value will be set. Cannot be null.</param>
    /// <param name="propertyName">The name of the public property to set. Must match the property name exactly, including case.</param>
    /// <param name="value">The value to assign to the property. The value must be compatible with the property's type.</param>
    public static void SetPropertyValue(this object obj, string propertyName, object value)
    {
        obj.GetType().GetProperty(propertyName)?.SetValue(obj, value);
    }

    /// <summary>
    /// Converts an object's public properties to a dictionary.
    /// </summary>
    /// <param name="obj"></param>
    /// <returns></returns>
    public static Dictionary<string, object?> ToDictionary(this object obj)
    {
        Dictionary<string, object?> dict = [];
        PropertyInfo[] properties = obj.GetType().GetProperties();
        foreach (var prop in properties)
        {
            dict[prop.Name] = prop.GetValue(obj);
        }
        return dict;
    }

    /// <summary>
    /// Determines if the object is of a numeric type.
    /// </summary>
    /// <returns></returns>
    public static bool IsNumericType(this object obj)
    {
        return obj is byte or sbyte
            or short or ushort
            or int or uint
            or long or ulong
            or float or double
            or decimal;
    }

    /// <summary>
    /// Determines if the object is contained within the specified values.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="obj"></param>
    /// <param name="values"></param>
    /// <returns></returns>
    public static bool In<T>(this T obj, params T[] values) => values.Contains(obj);

    /// <summary>
    /// Determines if the object is between the specified minimum and maximum values (inclusive).
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="obj"></param>
    /// <param name="min"></param>
    /// <param name="max"></param>
    /// <returns></returns>
    public static bool Between<T>(this T obj, T min, T max) where T : IComparable<T> =>
        obj.CompareTo(min) >= 0 && obj.CompareTo(max) <= 0;

    /// <summary>
    /// Dumps the public properties and their values of an object as a formatted string.
    /// </summary>
    /// <param name="obj"></param>
    /// <returns></returns>
    public static string DumpProperties(this object obj)
    {
        PropertyInfo[] properties = obj.GetType().GetProperties();
        StringBuilder sb = new();
        foreach (var prop in properties)
        {
            var value = prop.GetValue(obj);
            sb.AppendLine($"{prop.Name}: {value}");
        }
        return sb.ToString();
    }


    /// <summary>
    /// Creates a shallow clone of the source object by copying its public properties.
    /// </summary>
    /// <remarks>
    /// Shallow clone means that only the top-level properties are copied. If a property is a reference type,
    /// the reference is copied, not the actual object it points to.
    /// </remarks> 
    /// <typeparam name="T"></typeparam>
    /// <param name="source"></param>
    /// <returns></returns>
    /// <exception cref="ArgumentNullException"></exception>
    public static T CloneShallow<T>(this T source) where T : class, new()
    {
        ArgumentNullException.ThrowIfNull(source);

        T clone = new();
        PropertyInfo[] properties = typeof(T).GetProperties();
        foreach (PropertyInfo prop in properties)
        {
            if (prop.CanRead && prop.CanWrite)
            {
                var value = prop.GetValue(source);
                prop.SetValue(clone, value);
            }
        }
        return clone;
    }

    /// <summary>
    /// Creates a deep clone of the source object using JSON serialization.
    /// </summary>
    /// <remarks>
    /// Deep clone means that all nested objects are also cloned, resulting in a completely independent copy.
    /// </remarks>
    /// <typeparam name="T"></typeparam>
    /// <param name="source"></param>
    /// <returns></returns>
    /// <exception cref="ArgumentNullException"></exception>
    public static T CloneDeep<T>(this T source) where T : class
    {
        ArgumentNullException.ThrowIfNull(source);
        string serialized = JsonSerializer.GetSerializedString(source);
        return JsonSerializer.GetDeserializedObject<T>(serialized)!;
    }

    /// <summary>
    /// Determines if the object is null.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="obj"></param>
    /// <returns></returns>
    public static bool IsNull<T>(this T obj)
    {
        return obj is null;
    }

    /// <summary>
    /// Determines if the object is not null.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="obj"></param>
    /// <returns></returns>
    public static bool IsNotNull<T>(this T? obj)
    {
        return obj is not null;
    }

    /// <summary>
    /// Safely converts an object to its string representation, returning an empty string if the object is null.
    /// </summary>
    /// <param name="obj"></param>
    /// <returns></returns>
    public static string SafeToString(this object? obj)
    {
        return obj?.ToString() ?? string.Empty;
    }
}
