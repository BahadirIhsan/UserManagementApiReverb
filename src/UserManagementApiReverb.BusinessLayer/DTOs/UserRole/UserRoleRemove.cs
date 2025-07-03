namespace UserManagementApiReverb.BusinessLayer.DTOs.UserRole;

public class UserRoleRemove
{
    public Guid UserId { get; set; }
    public Guid RoleId { get; set; }
    public string? RemovedReason { get; set; } = string.Empty;
}