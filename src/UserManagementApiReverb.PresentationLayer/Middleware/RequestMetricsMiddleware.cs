using UserManagementApiReverb.BusinessLayer.CloudWatchMetricsService;

namespace UserManagementApiReverb.PresentationLayer.Middleware;

public class RequestMetricsMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ICloudWatchMetricsService _cloudWatchMetricsService;

    public RequestMetricsMiddleware(RequestDelegate next, ICloudWatchMetricsService cloudWatchMetricsService)
    {
        _next = next;
        _cloudWatchMetricsService = cloudWatchMetricsService;
    }

    public async Task Invoke(HttpContext context)
    {
        var endpoint = context.Request.Path.Value;
        
        await _cloudWatchMetricsService.SendRequestCountMetricAsync(endpoint!);
        
        await _next(context);
    }
}