using System;
using System.IO;
using DogsHouseService.Application.Middleware;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Moq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace DogsHouseService.Tests.Middleware;

public class ExceptionHandlingMiddlewareTests
{
    private readonly Mock<RequestDelegate> _nextMock;
    private readonly DefaultHttpContext _context;
    private readonly Mock<ILogger<ExceptionHandlingMiddleware>> _loggerMock;
    private readonly ExceptionHandlingMiddleware _middleware;

    public ExceptionHandlingMiddlewareTests()
    {
        _nextMock = new Mock<RequestDelegate>();
        _loggerMock = new Mock<ILogger<ExceptionHandlingMiddleware>>();
        _context = new DefaultHttpContext();
        _context.Response.Body = new MemoryStream();

        _middleware = new ExceptionHandlingMiddleware(_nextMock.Object, _loggerMock.Object);
    }

    [Fact]
    public async Task InvokeAsync_ShouldReturnInternalServerError_WhenExceptionThrown()
    {
        // Arrange
        _nextMock.Setup(n => n(It.IsAny<HttpContext>()))
                 .ThrowsAsync(new InvalidOperationException("Test exception"));

        // Act
        await _middleware.InvokeAsync(_context);

        // Assert
        _context.Response.StatusCode.Should().Be((int)HttpStatusCode.InternalServerError);
        _context.Response.ContentType.Should().Be("application/json");

        _context.Response.Body.Position = 0;
        using var reader = new StreamReader(_context.Response.Body, Encoding.UTF8);
        var body = await reader.ReadToEndAsync();

        body.Should().Contain("Internal Server Error");
        body.Should().Contain("Test exception");

        _loggerMock.VerifyLog(LogLevel.Error, Times.Once());
    }

    [Fact]
    public async Task InvokeAsync_ShouldContinuePipeline_WhenNoExceptionThrown()
    {
        // Arrange
        _nextMock.Setup(n => n(It.IsAny<HttpContext>()))
                 .Returns(Task.CompletedTask);

        // Act
        await _middleware.InvokeAsync(_context);

        // Assert
        _context.Response.StatusCode.Should().Be((int)HttpStatusCode.OK);
        _context.Response.Body.Length.Should().Be(0);
    }
}

// 🧩 Helper extension for verifying logger calls
public static class LoggerMockExtensions
{
    public static void VerifyLog<T>(
        this Mock<ILogger<T>> logger,
        LogLevel level,
        Times times)
    {
        logger.Verify(x => x.Log(
            level,
            It.IsAny<EventId>(),
            It.IsAny<It.IsAnyType>(),
            It.IsAny<Exception>(),
            (Func<It.IsAnyType, Exception?, string>)It.IsAny<object>()),
            times);
    }
}
