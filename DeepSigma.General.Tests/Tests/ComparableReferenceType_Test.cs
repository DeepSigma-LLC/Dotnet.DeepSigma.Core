using DeepSigma.Core.Tests.Models;
using Xunit;

namespace DeepSigma.Core.Tests.Tests;

public class ComparableReferenceType_Test
{
    [Fact]
    public void Test_ComparableReferenceType_ReferenceTypeShouldEqual()
    {
        var obj1 = new UniqueTestReferenceType { UniqueId = "id1" };
        var obj2 = new UniqueTestReferenceType { UniqueId = "id2" };
        var obj3 = obj1;
        Assert.True(obj1.Equals(obj3));
        Assert.False(obj1.Equals(obj2));
        Assert.True(obj1.GetHashCode() == obj3.GetHashCode());
        Assert.False(obj1.GetHashCode() == obj2.GetHashCode());
    }

    [Fact]
    public void Test_ComparableReferenceType_Should_Match()
    {
        var obj1 = new UniqueTestReferenceType { UniqueId = "id1" , Id = 2, Name = "Test"};
        var obj2 = new UniqueTestReferenceType { UniqueId = "id1", Id = 3 , Name = "Not Test"};
        Assert.True(obj1.Equals(obj2));
    }

    [Fact]
    public void Test_ComparableReferenceType_Should_Not_Match()
    {
        var obj1 = new UniqueTestReferenceType { UniqueId = "id1", Id = 2, Name = "Test" };
        var obj2 = new UniqueTestReferenceType { UniqueId = "id2", Id = 2, Name = "Test" };
        Assert.False(obj1.Equals(obj2));
    }

    [Fact]
    public void Test_ComparableReferenceType_Null_Comparison()
    {
        var obj1 = new UniqueTestReferenceType { UniqueId = "id1" };
        UniqueTestReferenceType? obj2 = null;
        Assert.False(obj1.Equals(obj2));
    }

    [Fact]
    public void Test_ComparableReferenceType_HashSet_Behavior()
    {
        var one = new UniqueTestReferenceType { UniqueId = "id1" };
        var two = new UniqueTestReferenceType { UniqueId = "id1" };
        HashSet<UniqueTestReferenceType> hashSet = [one];
        Assert.Contains(two, hashSet);
    }

    [Fact]
    public void Test_ComparableReferenceType_HashSet_ItemCount()
    {
        var one = new UniqueTestReferenceType { UniqueId = "id1" };
        var two = new UniqueTestReferenceType { UniqueId = "id1" };
        HashSet<UniqueTestReferenceType> hashSet = [one, two ];
        Assert.Single(hashSet);

        var three = new UniqueTestReferenceType { UniqueId = "id2" };
        hashSet.Add(three);
        Assert.Equal(2, hashSet.Count);
    }

}
