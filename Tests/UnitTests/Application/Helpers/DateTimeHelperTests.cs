using Application.Helpers;
using FluentAssertions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FCG.Tests.UnitTests.FCG.Tests.Application.Helpers
{
    public class DateTimeHelperTests
    {
        [Theory]
        [InlineData("E. South America Standard Time")] // São Paulo
        [InlineData("Pacific Standard Time")]          // Los Angeles
        [InlineData("GMT Standard Time")]              // Londres
        public void ConvertUtcToTimeZone_ShouldConvertDateTimeAndKeepCorrectKind(string timeZoneId)
        {
            // Arrange
            var utcDate = new DateTime(2025, 6, 1, 12, 0, 0, DateTimeKind.Utc);

            // Act
            var converted = DateTimeHelper.ConvertUtcToTimeZone(utcDate, timeZoneId);

            // Assert
            converted.Kind.Should().Be(DateTimeKind.Local);

            var tz = TimeZoneInfo.FindSystemTimeZoneById(timeZoneId);
            var expected = TimeZoneInfo.ConvertTimeFromUtc(utcDate, tz);
            converted.Should().Be(expected);
        }

        [Fact]
        public void ConvertUtcToTimeZone_ShouldThrowArgumentException_WhenDateIsNotUtc()
        {
            // Arrange
            var nonUtcDate = new DateTime(2025, 6, 1, 12, 0, 0, DateTimeKind.Local);

            // Act
            Action act = () => DateTimeHelper.ConvertUtcToTimeZone(nonUtcDate, "E. South America Standard Time");

            // Assert
            act.Should().Throw<ArgumentException>()
                .WithMessage("A data deve ser do tipo UTC.*")
                .And.ParamName.Should().Be("utcDateTime");
        }

        [Theory]
        [InlineData("E. South America Standard Time")] // São Paulo
        [InlineData("Pacific Standard Time")]          // Los Angeles
        [InlineData("GMT Standard Time")]              // Londres
        public void ConvertTimeZoneToUtc_ShouldConvertDateTimeAndReturnUtcKind(string timeZoneId)
        {
            // Arrange
            var localDate = new DateTime(2025, 6, 1, 9, 0, 0, DateTimeKind.Unspecified);

            // Act
            var utcDate = DateTimeHelper.ConvertTimeZoneToUtc(localDate, timeZoneId);

            // Assert
            utcDate.Kind.Should().Be(DateTimeKind.Utc);

            var tz = TimeZoneInfo.FindSystemTimeZoneById(timeZoneId);
            var expected = TimeZoneInfo.ConvertTimeToUtc(localDate, tz);
            utcDate.Should().Be(expected);
        }
    }
}
