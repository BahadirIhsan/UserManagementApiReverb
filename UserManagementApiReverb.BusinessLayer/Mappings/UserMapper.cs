using UserManagementApiReverb.BusinessLayer.DTOs.User;
using UserManagementApiReverb.Entities.Entities;

namespace UserManagementApiReverb.BusinessLayer.Mappings;

public class UserMapper : IUserMapper
{
    public UserResponse MapUserToUserResponse(User user)
    {
        if (user == null)
        {
            return null;
        }

        return new UserResponse()
        {
            UserId = user.UserId,
            Username = user.UserName,
            FirstName = user.FirstName,
            LastName = user.LastName,
            Email = user.Email
        };
    }
}