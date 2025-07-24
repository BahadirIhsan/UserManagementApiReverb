using Amazon.CloudWatch;
using Amazon.CloudWatch.Model;
using Microsoft.Extensions.Configuration;

namespace UserManagementApiReverb.BusinessLayer.CloudWatchMetricsService;

public class CloudWatchMetricsService : ICloudWatchMetricsService
{
    
    private readonly AmazonCloudWatchClient _client;
    private readonly string _namespace = "MyUserManagementApp/Security";

    public CloudWatchMetricsService(IConfiguration con)
    {
        var region = con["AWS:Region"];
        if (string.IsNullOrEmpty(region))
        {
            throw new Exception("AWS:Region değeri bulunamadı. appsettings dosyasını kontrol et.");
        }

        _client = new AmazonCloudWatchClient(Amazon.RegionEndpoint.GetBySystemName(region));
    }
    
    public async Task SendFailedLoginMetricAsync(string endpoint, string email)
    {
        
        Console.WriteLine("***************************************************************************************************************");

        var dimensions = new List<Dimension>();

        if (!string.IsNullOrWhiteSpace(endpoint))
            dimensions.Add(new Dimension { Name = "Endpoint", Value = endpoint });
        else
            dimensions.Add(new Dimension { Name = "Endpoint", Value = "Unknown" });

        if (!string.IsNullOrWhiteSpace(email))
            dimensions.Add(new Dimension { Name = "Email", Value = email });
        else
            dimensions.Add(new Dimension { Name = "Email", Value = "Unknown" });

        var req = new PutMetricDataRequest
        {
            Namespace = _namespace,
            MetricData = new List<MetricDatum>
            {
                new MetricDatum
                {
                    MetricName = "FailedLoginAttempts",
                    Value = 1,
                    Unit = StandardUnit.Count,
                    Timestamp = DateTime.UtcNow,
                    Dimensions = dimensions
                }
            }
        };
        
        
        await _client.PutMetricDataAsync(req);
        
    }

    public async Task SendRequestCountMetricAsync(string endpoint)
    {
        
        var dimensions = new List<Dimension>();

        if (!string.IsNullOrWhiteSpace(endpoint))
            dimensions.Add(new Dimension { Name = "Endpoint", Value = endpoint });
        else
            dimensions.Add(new Dimension { Name = "Endpoint", Value = "Unknown" });

        var req = new PutMetricDataRequest
        {
            Namespace = _namespace,
            MetricData = new List<MetricDatum>
            {
                new MetricDatum
                {
                    MetricName = "ApiRequestCount",
                    Value = 1,
                    Unit = StandardUnit.Count,
                    Timestamp = DateTime.UtcNow,
                    Dimensions = dimensions
                    
                }
            }
        };

        try
        {
            await _client.PutMetricDataAsync(req);
        }
        catch (Exception e)
        {
            Console.WriteLine($"Metric Gönderilemedi: {e.Message}");
        }
    }

    public async Task SendErrorMetricAsync(string endpoint, int statusCode)
    {
        var dimensions = new List<Dimension>();

        if (!string.IsNullOrWhiteSpace(endpoint))
            dimensions.Add(new Dimension { Name = "Endpoint", Value = endpoint });
        else
            dimensions.Add(new Dimension { Name = "Endpoint", Value = "Unknown" });
        
        if (statusCode > 0)
            dimensions.Add(new Dimension { Name = "StatusCode", Value = statusCode.ToString() });
        else
            dimensions.Add(new Dimension { Name = "StatusCode", Value = "Unknown" });
        
        
        var req = new PutMetricDataRequest
        {
            Namespace = _namespace,
            MetricData = new List<MetricDatum>
            {
                new MetricDatum
                {
                    MetricName = "ErrorCount",
                    Value = 1,
                    Unit = StandardUnit.Count,
                    Timestamp = DateTime.UtcNow,
                    Dimensions = dimensions
                }
            }
        };
        
        try
        {
            await _client.PutMetricDataAsync(req);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Hata metriği gönderilemedi: {ex.Message}");
        }
        
    }

    public async Task SendResponseTimeMetricAsync(string endpoint, int statusCode, long responseTimeMs)
    {
        var dimensions = new List<Dimension>();
        
        dimensions.Add(new Dimension { Name = "Endpoint", Value = string.IsNullOrWhiteSpace(endpoint) ? "Unknown" : endpoint });
        dimensions.Add(new Dimension { Name = "StatusCode", Value = statusCode.ToString() });
        
        var req = new PutMetricDataRequest
        {
            Namespace = _namespace,
            MetricData = new List<MetricDatum>
            {
                new MetricDatum
                {
                    MetricName = "ResponseTime",
                    Value = responseTimeMs,
                    Unit = StandardUnit.Milliseconds,
                    Timestamp = DateTime.UtcNow,
                    Dimensions = dimensions
                }
            }
        };

        await _client.PutMetricDataAsync(req);
        
    }
}