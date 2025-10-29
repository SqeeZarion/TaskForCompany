namespace DogsHouseService.Application.RateLimiting;

public class RateLimitOptions
{
    public int RequestsPerSecond { get; set; } = 10;
}