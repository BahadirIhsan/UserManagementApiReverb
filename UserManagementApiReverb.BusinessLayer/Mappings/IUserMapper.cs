using UserManagementApiReverb.BusinessLayer.DTOs.User;
using UserManagementApiReverb.Entities.Entities;

namespace UserManagementApiReverb.BusinessLayer.Mappings;

public interface IUserMapper
{
    UserResponse MapUserToUserResponse(User user);
}