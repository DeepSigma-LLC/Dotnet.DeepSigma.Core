using DeepSigma.Core.Extensions;
using Xunit;

namespace DeepSigma.Core.Tests.Tests.Extension
{
    public class DateTimeExtention_Tests
    {
        [Fact]
        public void Days360_Should_Be_30()
        {
            int result = DateTime.Days360(new DateTime(2023, 1, 1), new DateTime(2023, 1, 31));
            Assert.Equal(30, result);
        }

        [Fact]
        public void Days360_Feburary_Should_Be_27()
        {
            int result = DateTime.Days360(new DateTime(2023, 2, 1), new DateTime(2023, 2, 28));
            Assert.Equal(27, result);
        }

        [Fact]
        public void Days360_Feburary_Should_Be_30()
        {
            int result = DateTime.Days360(new DateTime(2023, 2, 1), new DateTime(2023, 3, 1));
            Assert.Equal(30, result);
        }

        [Fact]
        public void Days360_LeapYear_Should_Be_28()
        {
            int result = DateTime.Days360(new DateTime(2020, 2, 1), new DateTime(2020, 2, 29));
            Assert.Equal(28, result);
        }

        [Fact]
        public void Days360_LeapYear_Should_Be_30()
        {
            int result = DateTime.Days360(new DateTime(2020, 2, 1), new DateTime(2020, 3, 1));
            Assert.Equal(30, result);
        }
    }
}
