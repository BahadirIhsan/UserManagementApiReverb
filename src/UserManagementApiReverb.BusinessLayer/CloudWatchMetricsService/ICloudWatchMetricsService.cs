namespace UserManagementApiReverb.BusinessLayer.CloudWatchMetricsService;

public interface ICloudWatchMetricsService
{
    Task SendFailedLoginMetricAsync(string endpoint, string email);
}