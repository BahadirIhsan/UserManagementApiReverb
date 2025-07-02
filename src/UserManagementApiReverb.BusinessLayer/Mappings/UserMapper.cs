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

    public User MapFromRegisterRequest(UserRequestRegister req, string hash)
    {
        if (req == null)
        {
            return null;
        }

        return new User()
        {
            UserId = Guid.NewGuid(),
            UserName = req.Username,
            PasswordHash = hash,
            FirstName = req.FirstName,
            LastName = req.LastName,
            Email = req.Email,

            PhoneNumber = req.PhoneNumber,
            Birthday = req.Birthday,
            Address = req.Address,

            CreatedAt = DateTime.Now,
        };
    }
}