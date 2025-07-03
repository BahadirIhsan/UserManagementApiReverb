using UserManagementApiReverb.BusinessLayer.DTOs.UserRole;

namespace UserManagementApiReverb.BusinessLayer.UserRoleService;

public interface IUserRoleService
{
    Task AssignRoleAsync(UserRoleAssign req);
    Task<bool> RemoveRoleAsync(UserRoleRemove req);
    Task<List<UserRoleResponse>> GetRolesByUserIdAsync(Guid userId);
    Task<List<RoleUserResponse>> GetUsersByRoleIdAsync(Guid roleId);
    
}