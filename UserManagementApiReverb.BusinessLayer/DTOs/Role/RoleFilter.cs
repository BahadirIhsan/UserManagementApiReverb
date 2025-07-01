namespace UserManagementApiReverb.BusinessLayer.DTOs.Role;

public record RoleFilter
{
    public string? RoleName { get; set; }
    public string? RoleDescription { get; set; }
    public bool? IsSystemRole { get; set; }
}