namespace UserManagementApiReverb.BusinessLayer.Logging;

public interface IAppLogger
{
    void LogInfo(string message, object? data = null);
    void LogWarn(string message, object? data = null);
    void LogError(string message,Exception e, object? data = null);
}