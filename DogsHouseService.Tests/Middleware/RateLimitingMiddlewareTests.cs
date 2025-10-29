using DogsHouseService.Application.RateLimiting;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Moq;
using System.IO;
using System.Net;
using System.Threading.Tasks;
using Xunit;

namespace DogsHouseService.Tests.Middleware;

public class RateLimitingMiddlewareTests
{
    private readonly Mock<RequestDelegate> _nextMock;
    private readonly Mock<IRateLimiter> _rateLimiterMock;
    private readonly RateLimitingMiddleware _middleware;
    private readonly DefaultHttpContext _context;

    public RateLimitingMiddlewareTests()
    {
        _nextMock = new Mock<RequestDelegate>();
        _rateLimiterMock = new Mock<IRateLimiter>();
        _middleware = new RateLimitingMiddleware(_nextMock.Object, _rateLimiterMock.Object);
        _context = new DefaultHttpContext();
        _context.Response.Body = new MemoryStream();
    }

    [Fact]
    public async Task InvokeAsync_ShouldAllowRequest_WhenUnderLimit()
    {
        // Arrange
        _rateLimiterMock.Setup(r => r.ShouldAllowRequest()).Returns(true);

        // Act
        await _middleware.InvokeAsync(_context);

        // Assert
        _nextMock.Verify(n => n(It.IsAny<HttpContext>()), Times.Once);
        _context.Response.StatusCode.Should().Be((int)HttpStatusCode.OK);
    }

    [Fact]
    public async Task InvokeAsync_ShouldBlockRequest_WhenLimitExceeded()
    {
        // Arrange
        _rateLimiterMock.Setup(r => r.ShouldAllowRequest()).Returns(false);

        // Act
        await _middleware.InvokeAsync(_context);

        // Assert
        _nextMock.Verify(n => n(It.IsAny<HttpContext>()), Times.Never);
        _context.Response.StatusCode.Should().Be((int)HttpStatusCode.TooManyRequests);

        _context.Response.Body.Position = 0;
        using var reader = new StreamReader(_context.Response.Body);
        var body = await reader.ReadToEndAsync();
        body.Should().Contain("Too many requests");
    }
}