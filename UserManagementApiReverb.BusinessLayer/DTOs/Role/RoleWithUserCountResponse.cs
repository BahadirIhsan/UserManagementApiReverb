namespace UserManagementApiReverb.BusinessLayer.DTOs.Role;

public class RoleWithUserCountResponse
{
    public Guid RoleId { get; set; }
    public string RoleName { get; set; } = string.Empty;
    public string? RoleDescription { get; set; }
    public int UserCount { get; set; }
    public DateTime CreatedAt { get; set; }
}