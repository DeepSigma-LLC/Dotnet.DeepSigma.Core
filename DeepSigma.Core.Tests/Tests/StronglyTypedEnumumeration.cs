using Xunit;
using DeepSigma.Core.Tests.Models;

namespace DeepSigma.Core.Tests.Tests;

public class StronglyTypedEnumumeration
{
    [Fact]
    public void FromValue_Should_Return_Correct_Enumeration()
    {
        // Arrange
        var expected = CreditCard.Premium;
        // Act
        var actual = CreditCard.FromValue(2);
        // Assert
        Assert.Equal(expected, actual);
    }

    [Fact]
    public void FromName_Should_Return_Correct_Enumeration()
    {
        // Arrange
        var expected = CreditCard.Platinum;
        // Act
        var actual = CreditCard.FromName("platinum");
        // Assert
        Assert.Equal(expected, actual);
    }

    [Fact]
    public void FromValue_Should_Return_Null_For_Invalid_Value()
    {
        // Act
        var actual = CreditCard.FromValue(999);
        // Assert
        Assert.Null(actual);
    }

    [Fact]
    public void FromName_Should_Return_Null_For_Invalid_Name()
    {
        // Act
        var actual = CreditCard.FromName("invalid");
        // Assert
        Assert.Null(actual);
    }

    [Fact]
    public void FromName_Should_Be_Case_Insensitive()
    {
        // Arrange
        var expected = CreditCard.Standard;
        // Act
        var actual = CreditCard.FromName("STANDARD");
        // Assert
        Assert.Equal(expected, actual);
    }


    [Fact]
    public void Enumeration_Instances_Should_Be_Singleton()
    {
        // Arrange
        var expected = CreditCard.Standard;
        // Act
        var actual1 = CreditCard.FromValue(1);
        var actual2 = CreditCard.FromName("Standard");
        // Assert
        Assert.Same(expected, actual1);
        Assert.Same(expected, actual2);
    }

    [Fact]
    public void Enumeration_Instances_Should_Have_Correct_Discount()
    {
        // Arrange
        var expectedStandardDiscount = 0.01;
        var expectedPremiumDiscount = 0.03;
        var expectedPlatinumDiscount = 0.05;
        // Act
        var standardDiscount = CreditCard.Standard.Discount;
        var premiumDiscount = CreditCard.Premium.Discount;
        var platinumDiscount = CreditCard.Platinum.Discount;
        // Assert
        Assert.Equal(expectedStandardDiscount, standardDiscount);
        Assert.Equal(expectedPremiumDiscount, premiumDiscount);
        Assert.Equal(expectedPlatinumDiscount, platinumDiscount);
    }
}

