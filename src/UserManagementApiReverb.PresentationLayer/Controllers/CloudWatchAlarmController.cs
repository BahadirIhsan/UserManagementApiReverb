using Microsoft.AspNetCore.Mvc;
using UserManagementApiReverb.BusinessLayer.DTOs.Monitoring;
using UserManagementApiReverb.BusinessLayer.MonitoringAlarmService;

namespace UserManagementApiReverb.PresentationLayer.Controllers;
[ApiController]
[Route("api/[controller]")]
public class CloudWatchAlarmController : ControllerBase
{
    private readonly ICloudWatchAlarmService _alarmService;

    public CloudWatchAlarmController(ICloudWatchAlarmService alarmService)
    {
        _alarmService = alarmService;
    }

    [HttpPost("AlarmCreate")]
    public async Task<IActionResult> CreateAlarmAsync([FromBody] CloudWatchAlarmRequest request)
    {
        if(!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        try
        {
            await _alarmService.CreateAlarmAsync(request);
            return Ok(new {message = "Alarm created successfully."});
        }   
        catch (Exception e)
        {
            return StatusCode(500, new {message = "failed to create alarm", error = e.Message});
        }
    }
    
    [HttpGet]
    public async Task<IActionResult> GetAllAlarms()
    {
        var alarms = await _alarmService.GetAllAlarmsAsync();
        return Ok(alarms);
    }

    [HttpDelete("{alarmName}")]
    public async Task<IActionResult> DeleteAlarm(string alarmName)
    {
        await _alarmService.DeleteAlarmAsync(alarmName);
        return NoContent();
    }


}