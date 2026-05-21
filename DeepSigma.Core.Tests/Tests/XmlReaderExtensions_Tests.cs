using System.Text;
using System.Xml.Serialization;
using DeepSigma.Core.Utilities;
using Xunit;

namespace DeepSigma.Core.Tests.Tests;

public class XmlReaderExtensions_Tests
{
    [XmlRoot("item")]
    public sealed class ItemDto
    {
        [XmlElement("id")] public int Id { get; set; }
        [XmlElement("label")] public string Label { get; set; } = "";
    }

    private static MemoryStream StreamOf(string xml) => new(Encoding.UTF8.GetBytes(xml));

    [Fact]
    public async Task ReadElementsAsync_YieldsMatchingElements()
    {
        string xml = """
            <root>
              <item><id>1</id><label>one</label></item>
              <item><id>2</id><label>two</label></item>
              <item><id>3</id><label>three</label></item>
            </root>
            """;

        using var stream = StreamOf(xml);
        List<int> ids = new();
        await foreach (var element in XmlReaderExtensions.ReadElementsAsync(stream, "item"))
        {
            ids.Add(int.Parse(element.Element("id")!.Value));
        }

        Assert.Equal(new[] { 1, 2, 3 }, ids);
    }

    [Fact]
    public async Task ReadElementsAsync_Generic_DeserializesEachSubtree()
    {
        string xml = """
            <root>
              <item><id>10</id><label>alpha</label></item>
              <item><id>20</id><label>beta</label></item>
            </root>
            """;

        using var stream = StreamOf(xml);
        List<ItemDto> items = new();
        await foreach (var item in XmlReaderExtensions.ReadElementsAsync<ItemDto>(stream, "item"))
        {
            items.Add(item);
        }

        Assert.Equal(2, items.Count);
        Assert.Equal(10, items[0].Id);
        Assert.Equal("alpha", items[0].Label);
        Assert.Equal(20, items[1].Id);
        Assert.Equal("beta", items[1].Label);
    }

    [Fact]
    public async Task ReadElementsAsync_FiltersByNamespace()
    {
        // Two namespaces, only one should match.
        string xml = """
            <root xmlns:a="urn:a" xmlns:b="urn:b">
              <a:item>match-a</a:item>
              <b:item>skip-b</b:item>
              <a:item>match-a-2</a:item>
            </root>
            """;

        using var stream = StreamOf(xml);
        List<string> values = new();
        await foreach (var element in XmlReaderExtensions.ReadElementsAsync(stream, "item", namespaceUri: "urn:a"))
        {
            values.Add(element.Value);
        }

        Assert.Equal(new[] { "match-a", "match-a-2" }, values);
    }

    [Fact]
    public async Task ReadElementsAsync_NoMatches_YieldsNothing()
    {
        string xml = "<root><other/></root>";

        using var stream = StreamOf(xml);
        int count = 0;
        await foreach (var _ in XmlReaderExtensions.ReadElementsAsync(stream, "missing"))
        {
            count++;
        }

        Assert.Equal(0, count);
    }

    [Fact]
    public async Task ReadElementsAsync_HonorsCancellation()
    {
        // Build a large enough payload that we can cancel mid-iteration.
        var sb = new StringBuilder("<root>");
        for (int i = 0; i < 1000; i++) { sb.Append("<item><id>").Append(i).Append("</id><label>x</label></item>"); }
        sb.Append("</root>");

        using var stream = StreamOf(sb.ToString());
        using var cts = new CancellationTokenSource();
        int seen = 0;

        await Assert.ThrowsAsync<OperationCanceledException>(async () =>
        {
            await foreach (var _ in XmlReaderExtensions.ReadElementsAsync(stream, "item", cancellationToken: cts.Token))
            {
                seen++;
                if (seen == 5) { cts.Cancel(); }
            }
        });

        Assert.True(seen >= 5 && seen < 1000);
    }

    [Fact]
    public async Task ReadElementsAsync_ThrowsOnNullStream()
    {
        await Assert.ThrowsAsync<ArgumentNullException>(async () =>
        {
            await foreach (var _ in XmlReaderExtensions.ReadElementsAsync(null!, "item")) { }
        });
    }

    [Fact]
    public async Task ReadElementsAsync_ThrowsOnEmptyLocalName()
    {
        using var stream = StreamOf("<root/>");
        await Assert.ThrowsAsync<ArgumentException>(async () =>
        {
            await foreach (var _ in XmlReaderExtensions.ReadElementsAsync(stream, "")) { }
        });
    }
}
