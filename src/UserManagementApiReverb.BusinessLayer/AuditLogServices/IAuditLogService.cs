namespace UserManagementApiReverb.BusinessLayer.AuditLogServices;

public interface IAuditLogService
{
    Task LogAsync(string TableName, string Action, string? OldValue, string? NewValue, Guid? UserId);
}