namespace UserManagementApiReverb.BusinessLayer.Logging;

public interface IAppLogger
{
    void LogDebug(string message, string category = LogCategories.Application, object? data = null);
    void LogInfo(string message, string category = LogCategories.Application, object? data = null);
    void LogWarn(string message, string category = LogCategories.Application, object? data = null);
    void LogError(string message,Exception e, string category = LogCategories.Application, object? data = null);
    void LogCritical(string message, Exception e, string category = LogCategories.Application, object? data = null);
}