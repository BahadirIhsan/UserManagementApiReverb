using UserManagementApiReverb.BusinessLayer.DTOs.Role;
using UserManagementApiReverb.Entities.Entities;

namespace UserManagementApiReverb.BusinessLayer.RoleServices;

public interface IRoleService
{
    Task<RoleResponse> GetRoleByRoleIdAsync(Guid roleId);
    Task<RoleResponse> GetRoleByNameAsync(string roleName);
    Task<RoleResponse> CreateRoleAsync(RoleCreateRequest req);
    Task<RoleResponse> UpdateRoleAsync(Guid roleId, RoleUpdateRequest req);
    Task<bool> DeleteRoleAsync(Guid roleId);
    
    // buradaki isSystemRole ifadesinin amacı sadece systemRole'lerini çağrıabilir true girersek eğer false girersek olmayanları getirir.
    // null olarak bırakırsak tamamını getirir.
    Task<IEnumerable<RoleWithUserCountResponse>> GetAllRolesAsync(bool? IsSystemRole = null); 
}