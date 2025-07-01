using UserManagementApiReverb.BusinessLayer.DTOs.Role;
using UserManagementApiReverb.Entities.Entities;

namespace UserManagementApiReverb.BusinessLayer.Mappings;

public interface IRoleMapper
{
    RoleResponse MapRoleToResponse(Role role);
    RoleWithUserCountResponse MapRoleWithUserCountToResponse(Role role);
}