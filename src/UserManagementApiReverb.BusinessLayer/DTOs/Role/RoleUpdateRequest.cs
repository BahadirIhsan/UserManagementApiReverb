namespace UserManagementApiReverb.BusinessLayer.DTOs.Role;

public class RoleUpdateRequest
{
    public Guid Id { get; set; }
    public string RoleName { get; set; }
    public string? RoleDescription { get; set; }
}