namespace UserManagementApiReverb.BusinessLayer.Logging;

public class ActiveUserStore
{
    private readonly Dictionary<string, DateTime> _userLastSeen = new();
    private readonly object _lock = new();

    public void MarkUserActive(string userId)
    {
        lock (_lock)
        {
            _userLastSeen[userId] = DateTime.UtcNow;
        }        
    }
    
    // belirli bir süre içinde aktif olanları saydığımız method
    public int GetActiveUserCount(TimeSpan activeThreshold)
    {
        var now = DateTime.UtcNow;
        lock (_lock)
        {
            return _userLastSeen.Count(kvp => now - kvp.Value <= activeThreshold);
        }
    }
    
    // süresi geçenleri siliyoruz.
    public void CleanupExpiredUsers(TimeSpan maxInactive)
    {
        var now = DateTime.UtcNow;
        lock (_lock)
        {
            var expiredUsers = _userLastSeen
                .Where(kvp => now - kvp.Value > maxInactive)
                .Select(kvp => kvp.Key)
                .ToList();

            foreach (var userId in expiredUsers)
            {
                _userLastSeen.Remove(userId);
            }
        }
    }
    
    
}