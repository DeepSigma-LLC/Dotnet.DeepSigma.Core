using System.Runtime.CompilerServices;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Serialization;

namespace DeepSigma.Core.Utilities;

/// <summary>
/// Streaming XML helpers. Yield elements one at a time from a large XML stream without
/// materializing the full document. Intended for harvest/dump-style payloads
/// (OAI-PMH ListRecords, RSS feeds, S3 inventory, large XML data dumps, etc.).
/// </summary>
public static class XmlReaderExtensions
{
    /// <summary>
    /// Streams <see cref="XElement"/>s matching <paramref name="localName"/> (and optionally
    /// <paramref name="namespaceUri"/>) from <paramref name="xmlStream"/>. Each yielded element
    /// is detached and projection-ready; the underlying reader does not hold onto previous matches.
    /// </summary>
    public static async IAsyncEnumerable<XElement> ReadElementsAsync(
        Stream xmlStream,
        string localName,
        string? namespaceUri = null,
        [EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(xmlStream);
        ArgumentException.ThrowIfNullOrWhiteSpace(localName);

        XmlReaderSettings settings = new() { Async = true, IgnoreWhitespace = true, IgnoreComments = true };
        using XmlReader reader = XmlReader.Create(xmlStream, settings);

        while (await reader.ReadAsync().ConfigureAwait(false))
        {
            cancellationToken.ThrowIfCancellationRequested();

            if (reader.NodeType != XmlNodeType.Element) { continue; }
            if (reader.LocalName != localName) { continue; }
            if (namespaceUri is not null && reader.NamespaceURI != namespaceUri) { continue; }

            // ReadSubtree() returns a reader scoped to the current element; XNode.ReadFrom advances it.
            using XmlReader subtree = reader.ReadSubtree();
            // ReadFrom requires the reader to be positioned on an element.
            subtree.Read();
            XElement element = (XElement)XNode.ReadFrom(subtree);
            yield return element;
        }
    }

    /// <summary>
    /// Streams elements matching <paramref name="localName"/> and deserializes each into
    /// <typeparamref name="T"/> via <see cref="XmlSerializer"/>.
    /// </summary>
    public static async IAsyncEnumerable<T> ReadElementsAsync<T>(
        Stream xmlStream,
        string localName,
        string? namespaceUri = null,
        [EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        XmlSerializer serializer = new(typeof(T));

        await foreach (XElement element in ReadElementsAsync(xmlStream, localName, namespaceUri, cancellationToken)
                           .ConfigureAwait(false))
        {
            using XmlReader elementReader = element.CreateReader();
            T? value = (T?)serializer.Deserialize(elementReader);
            if (value is not null) { yield return value; }
        }
    }
}
