using Xunit;
using DeepSigma.Core;
using DeepSigma.Core.Tests.Models;

namespace DeepSigma.Core.Tests.Tests;

public class UniqueCollection
{
    [Fact]
    public void UniqueCollection_Should_Throw_On_Duplicate_Add()
    {
        UniqueCollection<UniqueTestReferenceType> collection = [];
        UniqueTestReferenceType item1 = new(){ UniqueId = "id1" };
        UniqueTestReferenceType item2 = new(){ UniqueId = "id1" };
        collection.Add(item1);
        Assert.Throws<InvalidOperationException>(() => collection.Add(item2));
    }

    [Fact]
    public void UniqueCollection_Should_Throw_On_Initialization()
    {
        UniqueTestReferenceType item1 = new(){ UniqueId = "id1" };
        UniqueTestReferenceType item2 = new(){ UniqueId = "id1" };
        Assert.Throws<InvalidOperationException>(() => { UniqueCollection<UniqueTestReferenceType> collection = [item1, item2]; });
    }

    [Fact]
    public void UniqueCollection_Should_Allow_Unique_Adds()
    {
        UniqueCollection<string> collection = [];
        collection.Add("Test");
        collection.Add("Test2");
        Assert.Equal(2, collection.Count());
    }
}
