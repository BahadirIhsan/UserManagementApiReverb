using UserManagementApiReverb.BusinessLayer.DTOs.User;

namespace UserManagementApiReverb.BusinessLayer.UserServices;

public interface IUserService
{
    Task<UserResponse> GetUserAsyncById(Guid UserId);
    Task<UserResponse> GetUserAsyncByEmail(string Email);
    Task<UserResponse> GetUserAsyncByUsername(string UserName);
}