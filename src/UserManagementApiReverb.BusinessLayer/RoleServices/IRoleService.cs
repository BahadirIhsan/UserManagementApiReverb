using UserManagementApiReverb.BusinessLayer.DTOs;
using UserManagementApiReverb.BusinessLayer.DTOs.Role;

namespace UserManagementApiReverb.BusinessLayer.RoleServices;

public interface IRoleService
{
    Task<RoleResponse> GetRoleByIdAsync(Guid roleId);
    Task<RoleResponse> GetRoleByNameAsync(string roleName);
    Task<RoleResponse> CreateRoleAsync(RoleCreateRequest req);
    Task<RoleResponse> UpdateRoleAsync(RoleUpdateRequest req);
    Task<bool> DeleteRoleAsync(Guid roleId);
    
    // buradaki isSystemRole ifadesinin amacı sadece systemRole'lerini çağrıabilir true girersek eğer false girersek olmayanları getirir.
    // null olarak bırakırsak tamamını getirir.
    Task<PagedResult<RoleWithUserCountResponse>> GetAllRolesPaginationAsync(Paging paging, Sorting sorting); 
}