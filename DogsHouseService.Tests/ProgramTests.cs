using FluentAssertions;
using Microsoft.AspNetCore.Builder;
using Xunit;

namespace DogsHouseService.Tests;

public class ProgramTests
{
    [Fact]
    public void Application_ShouldBuildSuccessfully()
    {
        var builder = WebApplication.CreateBuilder();
        builder.Services.Should().NotBeNull();
    }
}