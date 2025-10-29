namespace DogsHouseService.Application.RateLimiting;

public interface IRateLimiter
{
    bool ShouldAllowRequest();
}