using UserManagementApiReverb.BusinessLayer.DTOs.Role;
using UserManagementApiReverb.Entities.Entities;

namespace UserManagementApiReverb.BusinessLayer.Mappings;

public class RoleMapper : IRoleMapper
{
    public RoleResponse MapRoleToResponse(Role role)
    {
        if (role == null)
        {
            return null;
        }

        return new RoleResponse()
        {
            RoleId = role.RoleId,
            RoleName = role.RoleName,
            RoleDescription = role.RoleDescription,
            CreatedAt = role.CreatedAt
        };
    }

    public RoleWithUserCountResponse MapRoleWithUserCountToResponse(Role role)
    {
        if (role == null)
        {
            return null;
        }

        return new RoleWithUserCountResponse()
        {
            RoleId = role.RoleId,
            RoleName = role.RoleName,
            RoleDescription = role.RoleDescription,
            UserCount = role.UserRoles.Count, // burada UserRoles olarak verilen ifadenin interface tanımı içersinden direkt olarak geliyor buradaki count yapısı onun
            // sayesinde burada count olan ifadeyi çeke,bliyoruz yoksa kod tanımımızda böyle bir ifade bulunmuyor.
            CreatedAt = role.CreatedAt
        };
    }
}