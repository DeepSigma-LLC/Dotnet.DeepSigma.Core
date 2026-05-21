using System.Text;
using System.Xml.Linq;
using System.Xml.Serialization;
using DeepSigma.Core.Utilities;
using Xunit;

namespace DeepSigma.Core.Tests.Tests;

public class XmlUtilities_Tests
{
    [XmlRoot("person")]
    public sealed class PersonDto
    {
        [XmlElement("name")] public string Name { get; set; } = "";
        [XmlElement("age")] public int Age { get; set; }
    }

    [Fact]
    public void FromString_DeserializesXml()
    {
        string xml = "<person><name>Ada</name><age>36</age></person>";

        PersonDto? person = XMLUtilities.FromString<PersonDto>(xml);

        Assert.NotNull(person);
        Assert.Equal("Ada", person!.Name);
        Assert.Equal(36, person.Age);
    }

    [Fact]
    public void FromString_ReturnsDefault_ForEmptyInput()
    {
        Assert.Null(XMLUtilities.FromString<PersonDto>(""));
        Assert.Null(XMLUtilities.FromString<PersonDto>("   "));
    }

    [Fact]
    public async Task FromStreamAsync_DeserializesFromStream()
    {
        string xml = "<person><name>Grace</name><age>85</age></person>";
        using var stream = new MemoryStream(Encoding.UTF8.GetBytes(xml));

        PersonDto? person = await XMLUtilities.FromStreamAsync<PersonDto>(stream);

        Assert.NotNull(person);
        Assert.Equal("Grace", person!.Name);
        Assert.Equal(85, person.Age);
    }

    [Fact]
    public async Task FromStreamAsync_ThrowsOnNullStream()
    {
        await Assert.ThrowsAsync<ArgumentNullException>(
            () => XMLUtilities.FromStreamAsync<PersonDto>(null!));
    }

    [Fact]
    public async Task LoadDocumentAsync_ReturnsXDocument()
    {
        string xml = "<root><item id=\"1\"/><item id=\"2\"/></root>";
        using var stream = new MemoryStream(Encoding.UTF8.GetBytes(xml));

        XDocument doc = await XMLUtilities.LoadDocumentAsync(stream);

        Assert.NotNull(doc.Root);
        Assert.Equal("root", doc.Root!.Name.LocalName);
        Assert.Equal(2, doc.Root.Elements("item").Count());
    }

    [Fact]
    public void ToString_FromString_RoundTrips()
    {
        PersonDto original = new() { Name = "Linus", Age = 56 };

        string xml = XMLUtilities.ToString(original);
        PersonDto? roundtripped = XMLUtilities.FromString<PersonDto>(xml);

        Assert.NotNull(roundtripped);
        Assert.Equal(original.Name, roundtripped!.Name);
        Assert.Equal(original.Age, roundtripped.Age);
    }

    [Fact]
    public async Task SerializeToStreamAsync_WritesValidXml()
    {
        PersonDto original = new() { Name = "Margaret", Age = 84 };
        using var stream = new MemoryStream();

        await XMLUtilities.SerializeToStreamAsync(original, stream);

        stream.Position = 0;
        PersonDto? deserialized = await XMLUtilities.FromStreamAsync<PersonDto>(stream);
        Assert.NotNull(deserialized);
        Assert.Equal(original.Name, deserialized!.Name);
        Assert.Equal(original.Age, deserialized.Age);
    }

    [Fact]
    public async Task SerializeToStreamAsync_ThrowsOnNullDestination()
    {
        PersonDto person = new() { Name = "x", Age = 1 };
        await Assert.ThrowsAsync<ArgumentNullException>(
            () => XMLUtilities.SerializeToStreamAsync(person, null!));
    }

    [Fact]
    public void Existing_GetObject_StillWorks_AfterExtensions()
    {
        // Regression check: pre-existing file-path API must continue to work.
        string xml = "<person><name>Donald</name><age>86</age></person>";
        string path = Path.GetTempFileName();
        try
        {
            File.WriteAllText(path, xml, Encoding.UTF8);
            PersonDto? loaded = XMLUtilities.GetObject<PersonDto>(path);
            Assert.NotNull(loaded);
            Assert.Equal("Donald", loaded!.Name);
            Assert.Equal(86, loaded.Age);
        }
        finally
        {
            File.Delete(path);
        }
    }

    [Fact]
    public void Existing_Serialize_StillWorks_AfterExtensions()
    {
        // Regression check: pre-existing string Serialize API must continue to work.
        PersonDto person = new() { Name = "Alan", Age = 41 };
        string xml = XMLUtilities.Serialize(person);

        Assert.Contains("<name>Alan</name>", xml);
        Assert.Contains("<age>41</age>", xml);
    }
}
