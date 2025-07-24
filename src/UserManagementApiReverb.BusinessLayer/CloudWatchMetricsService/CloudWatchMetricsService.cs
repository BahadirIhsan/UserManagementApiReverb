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
}