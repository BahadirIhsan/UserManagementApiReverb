using UserManagementApiReverb.BusinessLayer.DTOs.LogDto_s;

namespace UserManagementApiReverb.BusinessLayer.Logging;

public interface ILogQueryService
{
    Task<IReadOnlyList<LogEntry>> QueryAsync(LogQueryRequest req, CancellationToken ct);
}