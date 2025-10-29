using DogsHouseService.Domain;
using FluentAssertions;
using Xunit;

namespace DogsHouseService.Tests.Domain;

public class DogTests
{
    [Fact]
    public void Should_CreateDog_WithProperties()
    {
        var dog = new Dog
        {
            Name = "Bolt",
            Color = "White",
            TailLength = 5,
            Weight = 10
        };

        dog.Name.Should().Be("Bolt");
        dog.Color.Should().Be("White");
        dog.TailLength.Should().Be(5);
        dog.Weight.Should().Be(10);
    }
}