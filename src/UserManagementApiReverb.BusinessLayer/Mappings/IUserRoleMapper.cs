using UserManagementApiReverb.BusinessLayer.DTOs.UserRole;
using UserManagementApiReverb.Entities.Entities;

namespace UserManagementApiReverb.BusinessLayer.Mappings;

public interface IUserRoleMapper
{
    UserRole MapFromAssignRequest(UserRoleAssign assign);
    UserRoleResponse MapUserRoleToResponse(UserRole userRole);
    RoleUserResponse MapRoleUserToResponse(UserRole userRole);

}