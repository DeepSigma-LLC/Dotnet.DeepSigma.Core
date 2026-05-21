using System.Xml;
using System.Xml.Linq;
using System.Xml.Serialization;

namespace DeepSigma.Core.Utilities;

/// <summary>
/// Utility class for XML serialization and deserialization. Supports both file-based and
/// in-memory / streaming workflows. The async helpers are HTTP-friendly and stream-aware.
/// </summary>
public class XMLUtilities
{
    /// <summary>
    /// Deserializes an object of type T from the specified XML file.
    /// </summary>
    public static T? GetObject<T>(string XMLFilePath)
    {
        using FileStream fileStream = new(XMLFilePath, FileMode.Open, FileAccess.Read, FileShare.Read);

        XmlSerializer serializer = new(typeof(T));
        return (T?)serializer.Deserialize(fileStream);
    }

    /// <summary>
    /// Serializes the specified object to its XML representation as a formatted string.
    /// </summary>
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

    /// <summary>
    /// Deserializes an object of type <typeparamref name="T"/> from an XML string.
    /// </summary>
    public static T? FromString<T>(string xml)
    {
        if (string.IsNullOrWhiteSpace(xml)) { return default; }

        XmlSerializer serializer = new(typeof(T));
        using var reader = new StringReader(xml);
        return (T?)serializer.Deserialize(reader);
    }

    /// <summary>
    /// Asynchronously deserializes an object of type <typeparamref name="T"/> from an XML stream.
    /// The stream is read via an async <see cref="XmlReader"/> so large payloads do not block.
    /// </summary>
    public static async Task<T?> FromStreamAsync<T>(Stream xml, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(xml);

        XmlReaderSettings settings = new() { Async = true, IgnoreWhitespace = true };
        using XmlReader reader = XmlReader.Create(xml, settings);

        // Advance to the root element asynchronously, then hand off to XmlSerializer.
        while (await reader.ReadAsync().ConfigureAwait(false))
        {
            if (reader.NodeType == XmlNodeType.Element) { break; }
            cancellationToken.ThrowIfCancellationRequested();
        }
        cancellationToken.ThrowIfCancellationRequested();
        if (reader.EOF) { return default; }

        XmlSerializer serializer = new(typeof(T));
        return (T?)serializer.Deserialize(reader);
    }

    /// <summary>
    /// Asynchronously loads an <see cref="XDocument"/> from a stream. Convenient for hand-projecting
    /// small XML responses (e.g. Atom feeds, S3 manifests) without committing to a serializer schema.
    /// </summary>
    public static Task<XDocument> LoadDocumentAsync(Stream xml, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(xml);
        return XDocument.LoadAsync(xml, LoadOptions.None, cancellationToken);
    }

    /// <summary>
    /// Serializes <paramref name="obj"/> to <paramref name="destination"/> as XML.
    /// </summary>
    public static async Task SerializeToStreamAsync<T>(T obj, Stream destination, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(destination);

        XmlWriterSettings settings = new() { Async = true, Indent = true };
        await using XmlWriter writer = XmlWriter.Create(destination, settings);

        XmlSerializer serializer = new(typeof(T));
        serializer.Serialize(writer, obj);
        await writer.FlushAsync().ConfigureAwait(false);
        cancellationToken.ThrowIfCancellationRequested();
    }

    /// <summary>
    /// Serializes <paramref name="obj"/> to an XML string. Companion to <see cref="FromString{T}(string)"/>.
    /// </summary>
    public static string ToString<T>(T obj, bool indent = true)
    {
        XmlSerializer serializer = new(typeof(T));
        using var stringWriter = new StringWriter();
        using XmlWriter writer = XmlWriter.Create(stringWriter, new XmlWriterSettings { Indent = indent });
        serializer.Serialize(writer, obj);
        return stringWriter.ToString();
    }
}
