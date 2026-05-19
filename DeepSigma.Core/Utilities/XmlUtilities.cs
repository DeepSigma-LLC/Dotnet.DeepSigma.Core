using System.Xml.Serialization;
using System.Xml;


namespace DeepSigma.Core.Utilities;

/// <summary>
/// Utility class for XML serialization and deserialization.
/// </summary>
public class XMLUtilities
{
    /// <summary>
    /// Deserializes an object of type T from the specified XML file.
    /// </summary>
    /// <remarks>The XML file must match the structure expected by the XmlSerializer for type T. If the file
    /// is not found or the XML is invalid, an exception may be thrown.</remarks>
    /// <typeparam name="T">The type of the object to deserialize from the XML file.</typeparam>
    /// <param name="XMLFilePath">The path to the XML file containing the serialized object. Must not be null or empty.</param>
    /// <returns>An instance of type T deserialized from the XML file, or null if the file does not contain a valid object of
    /// type T.</returns>
    public static T? GetObject<T>(string XMLFilePath)
    {
        using FileStream fileStream = new(XMLFilePath, FileMode.Open, FileAccess.Read, FileShare.Read);

        XmlSerializer serializer = new(typeof(T));
        return (T?)serializer.Deserialize(fileStream);
    }

    /// <summary>
    /// Serializes the specified object to its XML representation as a formatted string.
    /// </summary>
    /// <remarks>The returned XML is indented for readability. The type parameter must be compatible with
    /// XmlSerializer; types without a parameterless constructor or with unsupported members may cause serialization to
    /// fail.</remarks>
    /// <typeparam name="T">The type of the object to serialize. Must be serializable by the XmlSerializer.</typeparam>
    /// <param name="obj">The object to serialize to XML. Cannot be null.</param>
    /// <returns>A string containing the XML representation of the specified object.</returns>
    public static string Serialize<T>(T obj)
    {
        XmlSerializer serializer = new(typeof(T));
        using var string_writer = new StringWriter();
        using XmlTextWriter writer = new(string_writer)
        {
            Formatting = Formatting.Indented
        };

        serializer.Serialize(writer, obj);
        return string_writer.ToString();
    }
}
