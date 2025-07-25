using Amazon.CloudWatch.Model;

namespace UserManagementApiReverb.BusinessLayer.DTOs.Monitoring;

public class CloudWatchAlarmRequest
{
    public string AlarmName { get; set; } = default!;
    public string MetricName { get; set; } = default!;
    public string Namespace { get; set; } = "MyUserManagementApp/Security"; // default namespace
    public int Threshold { get; set; } // örneğin: 5 hata
    public int EvalutionPeriods { get; set; } = 1; // kaç kere eşik aşılmalı
    public int Period { get; set; } = 60; // saniye cinsinden (örn: 60s = 1dk)
    public string ComparisonOperator { get; set; } = "GreaterThanOrEqualToThreshold";
    public string Statistic { get; set; } = "Sum";
    public string? NotificationTopicArn { get; set; } // SNS topic ARN (opsiyonel)
    
    public List<Dimension> Dimensions { get; set; } = new();
    
    public string? Email { get; set; }
    
    
    /*
     {
         "alarmName": "string",
         "metricName": "FailedLoginAttempts",
         "namespace": "MyUserManagementApp/Security",
         "threshold": 5,
         "evalutionPeriods": 1,
         "period": 60,
         "comparisonOperator": "GreaterThanOrEqualToThreshold",
         "statistic": "Sum",
         "notificationTopicArn": "",
         "dimensions": [
           {
             "name": "Endpoint",
             "value": "/login"
           },
           {
             "name": "Email",
             "value": "string123"
           }
         ],
         "email": "bahadirihsan23@gmail.com"
       }
     */


}