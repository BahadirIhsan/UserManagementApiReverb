using UserManagementApiReverb.DataAccessLayer;
using UserManagementApiReverb.Entities.Entities;

namespace UserManagementApiReverb.BusinessLayer.AuditLogServices;

public class AuditLogService :  IAuditLogService 
{
    private readonly AppDbContext _db;

    public AuditLogService(AppDbContext db)
    {
        _db = db;
    }
    public async Task LogAsync(string TableName, string Action, string? OldValue, string? NewValue, Guid? UserId)
    {
        var log = new AuditLog
        {
            Id = Guid.NewGuid(),
            TableName = TableName,
            Action = Action,
            OldValues = OldValue,
            NewValues = NewValue,
            UserId = UserId,
            CreatedAt = DateTime.UtcNow
        };
        
        _db.AuditLogs.Add(log);
        await _db.SaveChangesAsync();
    }
}