
namespace UserManagementApiReverb.Entities.Entities;

public class UserRole
{
    public Guid UserId { get; set; }
    public Guid RoleId { get; set; }
    
    public User User { get; set; } = null!;
    public Role Role { get; set; } = null!;
    
    public DateTime AssignedAt { get; set; } = DateTime.UtcNow;
    public DateTime ExpiresAt { get; set; } = DateTime.Now;
    
    public DateTime? RemovedAt { get; set; } 
    public string? RemovedReason { get; set; }
}
