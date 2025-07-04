using Serilog;

namespace UserManagementApiReverb.BusinessLayer.Logging;

public class CloudWatchLogger : IAppLogger 
{
    private readonly ILogger _logger;

    public CloudWatchLogger(ILogger logger)
    {
        _logger = logger;
    }
    
    public void LogInfo(string message, object? data = null)
    {
        _logger.Information("{Message} {@Data}, message, data");
    }

    public void LogWarn(string message, object? data = null)
    {
        _logger.Warning("{Message} {@Data}, message, data");
    }

    public void LogError(string message, Exception e, object? data = null)
    {
        _logger.Error(e,"{Message} {@Data}", message, data);
    }
}