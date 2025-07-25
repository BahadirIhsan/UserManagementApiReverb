using System.Diagnostics;
using System.Security.Claims;
using UserManagementApiReverb.BusinessLayer.CloudWatchMetricsService;
using UserManagementApiReverb.BusinessLayer.Logging;

namespace UserManagementApiReverb.PresentationLayer.Middleware;

public class ActiveUserTrackingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ActiveUserStore _activeUserStore;
    private readonly ICloudWatchMetricsService _metricsService;

    public ActiveUserTrackingMiddleware(RequestDelegate next, ActiveUserStore activeUserStore, ICloudWatchMetricsService metricsService)
    {
        _next = next;
        _activeUserStore = activeUserStore;
        _metricsService = metricsService;
    }

    public async Task Invoke(HttpContext context)
    {
        var watch = Stopwatch.StartNew();
        await _next(context);
        watch.Stop();
        
        var elapsedMs = watch.ElapsedMilliseconds;
        var endpoint = context.Request.Path.Value ?? "Unknown";
        var statusCode = context.Response.StatusCode;

        try
        {
            if (context.User.Identity?.IsAuthenticated == true)
            {
                var userId = context.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                
                Console.WriteLine($"[Debug] Authenticated UserId: {userId}"); // deneme

                if (!string.IsNullOrEmpty(userId))
                {
                    _activeUserStore.MarkUserActive(userId);
                }
            }
            // deneme için yazıldı. Debugging
            if (context.User.Identity?.IsAuthenticated == true)
            {
                var userId = context.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

                Console.WriteLine($"✅ userId (NameIdentifier): {userId}");

                if (!string.IsNullOrEmpty(userId))
                {
                    _activeUserStore.MarkUserActive(userId);
                }
            }
            
            // response süresi cloudwatch'a gönderiliyor.
            // await _metricsService.SendResponseTimeMetricAsync(endpoint, statusCode, elapsedMs);

            var activeCount = _activeUserStore.GetActiveUserCount(TimeSpan.FromMinutes(5));
            await _metricsService.SendActiveUserCountMetricAsync(activeCount);
            
            _activeUserStore.CleanupExpiredUsers(TimeSpan.FromMinutes(30));
        }
        catch (Exception e)
        {
            Console.WriteLine($"[Metric Middleware Hatası] {e}");
        }
        
        Console.WriteLine($"[MetricData] Endpoint: {endpoint}, StatusCode: {statusCode}, Time: {elapsedMs}ms, ActiveUsers: {_activeUserStore.GetActiveUserCount(TimeSpan.FromMinutes(5))}");
        
    }
}