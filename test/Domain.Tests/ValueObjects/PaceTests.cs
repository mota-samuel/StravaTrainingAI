using Domain.ValueObjects;
using FluentAssertions;

namespace Domain.Tests.ValueObjects;
public class PaceTests
{
    [Fact]
    public void Calculate_Pace_Success()
    {
        //Arrange
        var distance = Distance.FromKm(5);
        var duration = TimeSpan.FromMinutes(25);

        //Act
        var pace = Pace.Calculate(distance, duration);

        //Assert
        pace.SecondesPerKm.Should().Be(300);
        pace.Minutes.Should().Be(5);
        pace.Seconds.Should().Be(0);
        pace.ToString().Should().Be("5:00/Km");
    }
}
