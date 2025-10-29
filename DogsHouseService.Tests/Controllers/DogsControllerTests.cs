using System;
using System.Collections.Generic;
using DogsHouseService.Application.DTOs;
using DogsHouseService.Application.Services;
using DogsHouseService.Controllers;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Moq;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace DogsHouseService.Tests.Controllers;

public class DogsControllerTests
{
    private readonly Mock<IDogService> _serviceMock;
    private readonly DogsController _controller;

    public DogsControllerTests()
    {
        _serviceMock = new Mock<IDogService>();
        _controller = new DogsController(_serviceMock.Object);
    }

    [Fact]
    public async Task GetDogs_ShouldReturnOk_WithDogList()
    {
        // Arrange
        var dogs = new List<DogResponse>
        {
            new DogResponse { Name = "A", Color = "Brown", TailLength = 10, Weight = 20 }
        };

        _serviceMock.Setup(s => s.GetAllAsync(null, null, 1, 10, It.IsAny<CancellationToken>()))
                    .ReturnsAsync(dogs);

        // Act
        var result = await _controller.GetDogs(null, null, 1, 10);

        // Assert
        result.Should().BeOfType<OkObjectResult>();
        var ok = (OkObjectResult)result;
        ok.Value.Should().BeEquivalentTo(dogs);
        ok.StatusCode.Should().Be(200);
    }

    [Fact]
    public async Task CreateDog_ShouldReturnOk_WhenValidRequest()
    {
        var request = new CreateDogRequest { Name = "Buddy", Color = "Black", TailLength = 8, Weight = 18 };
        var response = new DogResponse { Name = "Buddy", Color = "Black", TailLength = 8, Weight = 18 };

        _serviceMock.Setup(s => s.CreateAsync(request, It.IsAny<CancellationToken>()))
            .ReturnsAsync(response);

        var result = await _controller.CreateDog(request, CancellationToken.None);

        result.Should().BeOfType<CreatedResult>();
        var created = (CreatedResult)result;
        created.Value.Should().BeEquivalentTo(response);
    }


    [Fact]
    public async Task CreateDog_ShouldReturnBadRequest_WhenValidationFails()
    {
        // Arrange
        var request = new CreateDogRequest { Name = "", Color = "White", TailLength = 5, Weight = 10 };
        _serviceMock.Setup(s => s.CreateAsync(request, It.IsAny<CancellationToken>()))
                    .ThrowsAsync(new ArgumentException("Name cannot be empty."));

        // Act
        var result = await _controller.CreateDog(request, CancellationToken.None);

        // Assert
        result.Should().BeOfType<BadRequestObjectResult>();
        var bad = (BadRequestObjectResult)result;

        // Перевіряємо через серіалізацію
        var json = JsonSerializer.Serialize(bad.Value);
        json.Should().Contain("Name cannot be empty.");
    }

    [Fact]
    public async Task CreateDog_ShouldReturnConflict_WhenDogAlreadyExists()
    {
        var request = new CreateDogRequest { Name = "Rex", Color = "Brown", TailLength = 4, Weight = 12 };
        _serviceMock.Setup(s => s.CreateAsync(request, It.IsAny<CancellationToken>()))
            .ThrowsAsync(new InvalidOperationException("Dog with name 'Rex' already exists."));

        var result = await _controller.CreateDog(request, CancellationToken.None);

        result.Should().BeOfType<ConflictObjectResult>();
        var conflict = (ConflictObjectResult)result;

        var json = JsonSerializer.Serialize(conflict.Value);
        json.Should().Contain("Dog with name").And.Contain("already exists");
    }
}
