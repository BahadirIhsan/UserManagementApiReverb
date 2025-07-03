using UserManagementApiReverb.BusinessLayer.DTOs.UserRole;
using UserManagementApiReverb.Entities.Entities;

namespace UserManagementApiReverb.BusinessLayer.Mappings;

public class UserRoleMapper : IUserRoleMapper
{
    public UserRole MapFromAssignRequest(UserRoleAssign assign)
    {
        if (assign == null)
        {
            return null;
        }

        return new UserRole
        {
            UserId = assign.UserId,
            RoleId = assign.RoleId,
            AssignedAt = DateTime.Now,
            ExpiresAt = assign.ExpiresAt
        };
    }

    public UserRoleResponse MapUserRoleToResponse(UserRole userRole)
    {
        if (userRole == null)
        {
            return null;
        }

        return new UserRoleResponse
        {
            RoleId = userRole.RoleId,
            RoleName = userRole.Role.RoleName,
            RoleDescription = userRole.Role.RoleDescription,
            AssignedAt = userRole.AssignedAt,
            ExpiresAt = userRole.ExpiresAt,
            RemovedAt = userRole.RemovedAt,
            RemovedReason = userRole.RemovedReason
        };
    }

    public RoleUserResponse MapRoleUserToResponse(UserRole userRole)
    {
        if (userRole == null)
        {
            return null;
        }
        /*
    public Guid UserId { get; set; }
    public string UserName { get; set; }
    public string Email { get; set; }
    public string FullName { get; set; }
    public DateTime AssignedAt { get; set; }
        */
        return new RoleUserResponse
        {
            UserId = userRole.UserId,
            UserName = userRole.User.UserName,
            Email = userRole.User.Email,
            FullName = userRole.User.FirstName + " " + userRole.User.LastName,
            AssignedAt = userRole.AssignedAt
        };
    }
}