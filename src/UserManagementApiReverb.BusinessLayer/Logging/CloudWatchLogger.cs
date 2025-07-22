using Serilog;

namespace UserManagementApiReverb.BusinessLayer.Logging;

public class CloudWatchLogger : IAppLogger 
{
    private readonly ILogger _logger;

    public CloudWatchLogger(ILogger logger)
    {
        _logger = logger;
    }
    
    public void LogDebug(string message, string category = LogCategories.Application, object? data = null)
    {   
        _logger.ForContext("Category", category).Debug("{message} {@data}", message, data);
    }

    public void LogInfo(string message, string category = LogCategories.Application, object? data = null)
    {
        _logger.ForContext("Category", category).Information("{message} {@data}", message, data);
    }

    public void LogWarn(string message, string category = LogCategories.Application, object? data = null)
    {
        _logger.ForContext("Category", category).Warning("{message} {@data}", message, data);
    }

    public void LogError(string message, Exception e, string category = LogCategories.Application, object? data = null)
    {
        _logger.ForContext("Category", category).Error(e, "{message} {@data}", message, data);
    }

    public void LogCritical(string message, Exception e, string category = LogCategories.Application, object? data = null)
    {
        _logger.ForContext("Category", category).Fatal(e,"{message} {@data}", message, data);
    }
}