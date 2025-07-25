using UserManagementApiReverb.BusinessLayer.DTOs.Monitoring;

namespace UserManagementApiReverb.BusinessLayer.MonitoringAlarmService;

public interface ICloudWatchAlarmService
{ 
    Task CreateAlarmAsync(CloudWatchAlarmRequest request);
    Task <List<string>> GetAllAlarmsAsync();
    Task DeleteAlarmAsync(string alarmName);

}