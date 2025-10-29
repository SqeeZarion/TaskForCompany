using System.Threading.Tasks;
using DogsHouseService.Controllers;
using Microsoft.AspNetCore.Mvc;
using Xunit;
using FluentAssertions;

namespace DogsHouseService.Tests.Controllers
{
    public class PingControllerTests
    {
        [Fact]
        public void Get_ShouldReturnPong()
        {
            // Arrange
            var controller = new PingController();

            // Act
            var result = controller.Get();

            // Assert
            var okResult = result as OkObjectResult;
            okResult.Should().NotBeNull();
            okResult!.Value.Should().Be("Dogshouseservice.Version1.0.1");
        }
    }

}