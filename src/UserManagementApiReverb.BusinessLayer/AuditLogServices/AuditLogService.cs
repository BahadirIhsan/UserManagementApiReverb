using UserManagementApiReverb.DataAccessLayer;
using UserManagementApiReverb.Entities.Entities;

namespace UserManagementApiReverb.BusinessLayer.AuditLogServices;

// olurda interceptors ile otomatik loglamanın yetmeyeceği durumlarda manuel olarak tabloya işlemek istediğimiz bir şey
// olursa bu durumlar için yazılmış ufak bir sınıf ve method. öğrenmek için yazdım. Aslında ilk olarak bunu yazdım
// ve bu şekilde loglama yapmanının büyük bir amelelik olduğnu farkedince çünkü tüm crud işlemleri için methodların
// içinde tek tek bu methodu çağırıp içini doludrmamız gerekecekti hadi nesnenin içini bir mapper ile kolay
// bir şekilde doldursak bile tek tek her crud methoduna eklemek çok yorucu ve kod cleanliğini azaltan bir işlem
// olacağı için yapmadım ve devamında interceptors'u öğrendim.
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