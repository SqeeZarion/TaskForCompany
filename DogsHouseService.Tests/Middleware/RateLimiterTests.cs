using System;
using System.Threading;
using DogsHouseService.Application.RateLimiting;
using FluentAssertions;
using Xunit;

namespace DogsHouseService.Tests.RateLimiting;

public class RateLimiterTests
{
    [Fact]
    public void ShouldAllowRequest_UnderLimit()
    {
        // Arrange
        var limiter = new InMemoryRateLimiter(maxRequests: 3, window: TimeSpan.FromSeconds(1));

        // Act + Assert
        limiter.ShouldAllowRequest().Should().BeTrue();
        limiter.ShouldAllowRequest().Should().BeTrue();
        limiter.ShouldAllowRequest().Should().BeTrue();
    }

    [Fact]
    public void ShouldBlockRequest_WhenOverLimit()
    {
        // Arrange
        var limiter = new InMemoryRateLimiter(maxRequests: 2, window: TimeSpan.FromSeconds(1));

        // Act
        limiter.ShouldAllowRequest().Should().BeTrue();
        limiter.ShouldAllowRequest().Should().BeTrue();
        limiter.ShouldAllowRequest().Should().BeFalse();
    }

    [Fact]
    public void ShouldAllowAgain_AfterWindowExpires()
    {
        // Arrange
        var limiter = new InMemoryRateLimiter(maxRequests: 2, window: TimeSpan.FromMilliseconds(200));

        // Act
        limiter.ShouldAllowRequest().Should().BeTrue();
        limiter.ShouldAllowRequest().Should().BeTrue();
        limiter.ShouldAllowRequest().Should().BeFalse();

        // Wait for window to expire
        Thread.Sleep(250);

        // Assert — should allow again
        limiter.ShouldAllowRequest().Should().BeTrue();
    }
}