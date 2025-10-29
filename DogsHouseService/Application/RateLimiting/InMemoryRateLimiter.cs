using DogsHouseService.Application.RateLimiting;

public class InMemoryRateLimiter : IRateLimiter
{
    private readonly int _maxRequests;
    private readonly TimeSpan _window;
    private readonly Queue<DateTime> _timestamps = new();

    public InMemoryRateLimiter(int maxRequests, TimeSpan window)
    {
        _maxRequests = maxRequests;
        _window = window;
    }

    public bool ShouldAllowRequest()
    {
        var now = DateTime.UtcNow;

        lock (_timestamps)
        {
            while (_timestamps.Count > 0 && now - _timestamps.Peek() > _window)
                _timestamps.Dequeue();

            if (_timestamps.Count >= _maxRequests)
                return false;

            _timestamps.Enqueue(now);
            return true;
        }
    }
}