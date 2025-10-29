using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;

namespace DogsHouseService.Application.RateLimiting;

public class RateLimitingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly IRateLimiter _rateLimiter;

    public RateLimitingMiddleware(RequestDelegate next, IRateLimiter rateLimiter)
    {
        _next = next;
        _rateLimiter = rateLimiter;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        if (!_rateLimiter.ShouldAllowRequest())
        {
            context.Response.StatusCode = StatusCodes.Status429TooManyRequests;
            await context.Response.WriteAsJsonAsync(new
            {
                error = "Too many requests. Please try again later."
            });
            return;
        }

        await _next(context);
    }
}