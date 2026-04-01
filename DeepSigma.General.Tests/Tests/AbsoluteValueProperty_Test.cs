using Xunit;
using DeepSigma.General.Tests.Models;

namespace DeepSigma.General.Tests.Tests;

public class AbsoluteValueProperty_Test
{
    [Fact]
    public void AbsoluteValueProperty_SetNegativeValue_ValueIsAbsolute()
    {
        AbsoluteValue<int> absoluteValue = new(-10);
        Assert.Equal(10, absoluteValue.Value);
    }

    [Fact]
    public void AbsoluteValueProperty_SetPositiveValue_ValueRemainsUnchanged()
    {
        AbsoluteValue<int> absoluteValue = new(10);
        Assert.Equal(10, absoluteValue.Value);
    }

    [Fact]
    public void AbsoluteValueProperty_SetZeroValue_ValueRemainsZero()
    {
        AbsoluteValue<decimal> absoluteValue = 0;
        Assert.Equal(0, absoluteValue.Value);
    }


    [Fact]
    public void AbsoluteValueProperty_TradeQuantity_SetNegativeValue_ValueIsAbsolute()
    {
        TradeTest trade = new()
        {
            Quantity = -50m
        };
        Assert.Equal(50m, trade.Quantity.Value);
    }
}
