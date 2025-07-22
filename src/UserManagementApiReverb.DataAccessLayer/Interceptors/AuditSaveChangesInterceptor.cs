using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using System.Text.Json;
using Microsoft.AspNetCore.Http;
using UserManagementApiReverb.Entities.Entities;

namespace UserManagementApiReverb.DataAccessLayer.Interceptors;

public class AuditSaveChangesInterceptor : SaveChangesInterceptor
{
    private readonly IHttpContextAccessor _http;

    public AuditSaveChangesInterceptor(IHttpContextAccessor http)
    {
        _http = http;
    }

    public async override ValueTask<InterceptionResult<int>> SavingChangesAsync(
        DbContextEventData eventData, 
        InterceptionResult<int> result,
        CancellationToken cancellationToken = default)
    {
        var context = eventData.Context;
        var UserId = GetUserId();

        // changeTracker ile değişiklik olan tüm yapılar burada tutuluyor. gerek güncelleme gerek silme gerek ekleme olarak.
        var entries = context.ChangeTracker.Entries().ToList(); // Koleksiyonu sabitliyoruz

        foreach (var entry in entries
                     .Where(e => e.State == EntityState.Modified ||  
                                 e.State == EntityState.Added ||
                                 e.State == EntityState.Deleted))
        {
            var log = new AuditLog
            {
                Id = Guid.NewGuid(),
                TableName = entry.Metadata.GetTableName() ?? "unknown",
                Action = entry.State.ToString(),
                UserId = UserId,
                CreatedAt = DateTime.UtcNow,
                OldValues = entry.State == EntityState.Modified || entry.State == EntityState.Deleted
                    ? JsonSerializer.Serialize(entry.OriginalValues.ToObject())
                    : null,
                NewValues = entry.State == EntityState.Modified || entry.State == EntityState.Added
                    ? JsonSerializer.Serialize(entry.CurrentValues.ToObject())
                    : null
            };

            context.Add(log);
        }
        
        return await base.SavingChangesAsync(eventData, result, cancellationToken);
    }
    
    private Guid? GetUserId()
    {
        var userIdStr = _http.HttpContext?.User?.FindFirst("sub")?.Value;
        return Guid.TryParse(userIdStr, out var userId) ? userId : null;
    }
}