using Amazon.CloudWatch;
using Amazon.CloudWatch.Model;
using UserManagementApiReverb.BusinessLayer.DTOs.Monitoring;
using UserManagementApiReverb.BusinessLayer.NotificationServices;

namespace UserManagementApiReverb.BusinessLayer.MonitoringAlarmService;

public class CloudWatchAlarmService :  ICloudWatchAlarmService
{
    private readonly IAmazonCloudWatch _cloudWatch;
    private readonly SnsService _snsService;


    public CloudWatchAlarmService(IAmazonCloudWatch cloudWatch, SnsService snsService)
    {
        _cloudWatch = cloudWatch;
        _snsService = snsService;
    }
    
    public async Task CreateAlarmAsync(CloudWatchAlarmRequest request)
    {
        
        // 1. SNS Topic oluştur
        var topicName = request.AlarmName + "_Topic";
        var topicArn = await _snsService.CreateTopicAsync(topicName);
        
        // 2. E-mail ile abone ol
        if (!string.IsNullOrWhiteSpace(request.Email))
        {
            await _snsService.SubscribeEmailAsync(topicArn, request.Email);
        }
        
        // 3. Dimensions listesi dönüştürülüyor
        List<Dimension>? dimensions = request.Dimensions?.Select(d =>
            new Dimension { Name = d.Name, Value = d.Value }).ToList();
        
        var alarmRequest = new PutMetricAlarmRequest
        {
            AlarmName = request.AlarmName,
            MetricName = request.MetricName,
            Namespace = request.Namespace,
            Threshold = request.Threshold,
            EvaluationPeriods = request.EvalutionPeriods,
            Period = request.Period,
            ComparisonOperator = request.ComparisonOperator,
            Statistic = request.Statistic,
            Dimensions = request.Dimensions, // kritik kısım
            ActionsEnabled = true,
            AlarmActions = new List<string> { topicArn },
            AlarmDescription = $"Alarm for {request.MetricName}"
        };
        
        await _cloudWatch.PutMetricAlarmAsync(alarmRequest);
    }
    
    public async Task<List<string>> GetAllAlarmsAsync()
    {
        var request = new DescribeAlarmsRequest();
        var response = await _cloudWatch.DescribeAlarmsAsync(request);

        return response.MetricAlarms.Select(a => a.AlarmName).ToList();
    }
    
    public async Task DeleteAlarmAsync(string alarmName)
    {
        var request = new DeleteAlarmsRequest
        {
            AlarmNames = new List<string> { alarmName }
        };

        await _cloudWatch.DeleteAlarmsAsync(request);
    }

}