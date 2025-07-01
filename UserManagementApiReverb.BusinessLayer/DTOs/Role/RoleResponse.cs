namespace UserManagementApiReverb.BusinessLayer.DTOs.Role;

public class RoleResponse
{
    public Guid RoleId { get; set; }
    public string RoleName { get; set; } = string.Empty;
    public string? RoleDescription { get; set; }
    public DateTime CreatedAt { get; set; }
}