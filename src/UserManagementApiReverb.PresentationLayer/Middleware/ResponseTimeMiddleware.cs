using System.Diagnostics;
using UserManagementApiReverb.BusinessLayer.CloudWatchMetricsService;

namespace UserManagementApiReverb.PresentationLayer.Middleware;

public class ResponseTimeMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ICloudWatchMetricsService  _cloudWatchMetrics;

    public ResponseTimeMiddleware(RequestDelegate next, ICloudWatchMetricsService  cloudWatchMetrics)
    {
        _next = next;
        _cloudWatchMetrics = cloudWatchMetrics;
    }

    public async Task Invoke(HttpContext context)
    {
        
        Console.WriteLine(">>> SendResponseTimeMetricAsync çağrıldı");

        var watch = Stopwatch.StartNew();
        await _next(context);
        watch.Stop();
        
        var elapsedMs = watch.ElapsedMilliseconds;
        var endpoint = context.Request.Path.Value ?? "Unknown";
        var statusCode = context.Response.StatusCode;

        try
        {
            await _cloudWatchMetrics.SendResponseTimeMetricAsync(endpoint, statusCode, elapsedMs);
        }
        catch (Exception e)
        {
            Console.WriteLine($"Endpoint suresi hesaplanirken exception cikti. {e.Message}");
        }
        
        Console.WriteLine($"[MetricData] Endpoint: {endpoint}, StatusCode: {statusCode}, Time: {elapsedMs}ms");

    }
    
}