namespace UserManagementApiReverb.BusinessLayer.DTOs.UserRole;

public class RoleUserResponse
{
    public Guid UserId { get; set; }
    public string UserName { get; set; }
    public string Email { get; set; }
    public string FullName { get; set; }
    public DateTime AssignedAt { get; set; }
    
}