namespace UserManagementApiReverb.BusinessLayer.DTOs.UserRole;

public class UserRoleResponse
{
    public Guid RoleId { get; set; }
    public string RoleName { get; set; }
    public string? RoleDescription { get; set; }
    public DateTime AssignedAt { get; set; }
    public DateTime? ExpiresAt { get; set; }
    public DateTime? RemovedAt { get; set; }
    public string? RemovedReason { get; set; }
}