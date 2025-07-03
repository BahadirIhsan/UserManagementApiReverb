namespace UserManagementApiReverb.BusinessLayer.DTOs.UserRole;

public class UserRoleAssign
{
    public Guid UserId { get; set; }
    public Guid RoleId { get; set; }
    public DateTime ExpiresAt { get; set; }
}