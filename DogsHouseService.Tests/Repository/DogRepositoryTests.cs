using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using DogsHouseService.Domain;
using DogsHouseService.Infrastructure;
using DogsHouseService.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using Xunit;
using FluentAssertions;

namespace DogsHouseService.Tests.Repository;

public class DogRepositoryTests
{
    private readonly DogsDbContext _context;
    private readonly DogRepository _repository;

    public DogRepositoryTests()
    {
        var options = new DbContextOptionsBuilder<DogsDbContext>()
            .UseInMemoryDatabase("TestDb_" + Guid.NewGuid())
            .Options;

        _context = new DogsDbContext(options);
        _repository = new DogRepository(_context);
    }
    

    [Fact]
    public async Task AddAsync_ShouldAddDogToDatabase()
    {
        // Arrange
        var dog = new Dog { Name = "Lucky", Color = "Brown", TailLength = 10, Weight = 20 };

        // Act
        await _repository.AddAsync(dog);
        await _context.SaveChangesAsync();

        // Assert
        var result = await _context.Dogs.FirstOrDefaultAsync(d => d.Name == "Lucky");
        result.Should().NotBeNull();
        result!.Color.Should().Be("Brown");
    }

    [Fact]
    public async Task GetAllAsync_ShouldReturnAllDogs()
    {
        // Arrange
        var dogs = new List<Dog>
        {
            new() { Name = "Max", Color = "Black", TailLength = 8, Weight = 25 },
            new() { Name = "Bella", Color = "White", TailLength = 9, Weight = 18 }
        };

        await _context.Dogs.AddRangeAsync(dogs);
        await _context.SaveChangesAsync();

        // Act
        var result = await _repository.GetAllAsync(null, null, 1, 10);

        // Assert
        result.Should().NotBeNull();
        result.Count.Should().Be(2);
    }

    [Fact]
    public async Task DeleteAsync_ShouldRemoveDogFromDatabase()
    {
        // Arrange
        var dog = new Dog { Name = "Rex", Color = "Grey", TailLength = 7, Weight = 15 };
        await _context.Dogs.AddAsync(dog);
        await _context.SaveChangesAsync();

        // Act
        await _repository.DeleteAsync(dog);

        // Assert
        var exists = await _context.Dogs.AnyAsync(d => d.Name == "Rex");
        exists.Should().BeFalse();
    }

    [Fact]
    public async Task ExistsByNameAsync_ShouldReturnTrue_IfDogExists()
    {
        // Arrange
        var dog = new Dog { Name = "Buddy", Color = "Golden", TailLength = 12, Weight = 22 };
        await _context.Dogs.AddAsync(dog);
        await _context.SaveChangesAsync();

        // Act
        var exists = await _repository.ExistsByNameAsync("Buddy");

        // Assert
        exists.Should().BeTrue();
    }

    [Fact]
    public async Task ExistsByNameAsync_ShouldReturnFalse_IfDogDoesNotExist()
    {
        // Act
        var exists = await _repository.ExistsByNameAsync("GhostDog");

        // Assert
        exists.Should().BeFalse();
    }
}
