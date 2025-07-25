using UserManagementApiReverb.Entities.Entities;

namespace UserManagementApiReverb.DataAccessLayer.Repositories.Interfaces;

public interface IUserRoleRepository
{
    Task<bool> UserRoleExistsAsync(Guid userId, Guid roleId);
    Task AddUserRoleAsync(UserRole userRole);
    Task RemoveUserRoleAsync(UserRole userRole);
    Task<List<UserRole>> GetRolesByUserIdAsync(Guid userId);
    Task<List<UserRole>> GetUsersByRoleIdAsync(Guid roleId);
    Task<List<UserRole>> GetUserRolesAsync(Guid userId);

}