using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using DogsHouseService.Application.DTOs;
using DogsHouseService.Application.Services;
using DogsHouseService.Domain;
using DogsHouseService.Infrastructure.Repositories;
using FluentAssertions;
using Moq;
using Xunit;

namespace DogsHouseService.Tests.Services;

public class DogServiceTests
{
    private readonly Mock<IDogRepository> _repoMock;
    private readonly DogService _service;

    public DogServiceTests()
    {
        _repoMock = new Mock<IDogRepository>();
        _service = new DogService(_repoMock.Object);
    }

    [Fact]
    public async Task CreateAsync_ShouldCreateDog_WhenValidInput()
    {
        // Arrange
        var request = new CreateDogRequest
        {
            Name = "Barky",
            Color = "Brown",
            TailLength = 15,
            Weight = 20
        };

        _repoMock.Setup(r => r.ExistsByNameAsync(request.Name, It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        _repoMock.Setup(r => r.AddAsync(It.IsAny<Dog>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        // Act
        var result = await _service.CreateAsync(request, CancellationToken.None);

        // Assert
        result.Name.Should().Be("Barky");
        result.Color.Should().Be("Brown");
        result.TailLength.Should().Be(15);
        result.Weight.Should().Be(20);

        _repoMock.Verify(r => r.AddAsync(It.IsAny<Dog>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task CreateAsync_ShouldThrow_WhenDogExists()
    {
        // Arrange
        var request = new CreateDogRequest
        {
            Name = "Existing",
            Color = "Gray",
            TailLength = 10,
            Weight = 12
        };

        _repoMock.Setup(r => r.ExistsByNameAsync(request.Name, It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        // Act
        var act = async () => await _service.CreateAsync(request, CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<InvalidOperationException>()
            .WithMessage("Dog with name 'Existing' already exists.");
    }

    [Fact]
    public async Task GetAllAsync_ShouldReturnMappedDogs()
    {
        // Arrange
        var dogs = new List<Dog>
        {
            new Dog { Name = "Rex", Color = "Black", TailLength = 20, Weight = 30 },
            new Dog { Name = "Spot", Color = "White", TailLength = 15, Weight = 25 }
        };

        _repoMock.Setup(r => r.GetAllAsync(null, null, 1, 10, It.IsAny<CancellationToken>()))
            .ReturnsAsync(dogs);

        // Act
        var result = await _service.GetAllAsync(null, null, 1, 10, CancellationToken.None);

        // Assert
        result.Should().HaveCount(2);
        result.First().Name.Should().Be("Rex");
    }
    
    [Fact]
    public async Task CreateAsync_ShouldThrow_WhenTailLengthOrWeightNegative()
    {
        // Arrange
        var invalidRequest = new CreateDogRequest
        {
            Name = "BadDog",
            Color = "Red",
            TailLength = -5,
            Weight = -10
        };

        // Act
        var act = async () => await _service.CreateAsync(invalidRequest, CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<ArgumentException>()
            .WithMessage("Tail length must be non-negative.");
    }
}

