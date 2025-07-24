namespace UserManagementApiReverb.BusinessLayer.CloudWatchMetricsService;

public interface ICloudWatchMetricsService
{
    Task SendFailedLoginMetricAsync(string endpoint, string email);
    Task SendRequestCountMetricAsync(string endpoint);
    Task SendErrorMetricAsync(string endpoint, int statusCode);
    Task SendResponseTimeMetricAsync(String endpoint, int statusCode, long responseTimeMs);

}