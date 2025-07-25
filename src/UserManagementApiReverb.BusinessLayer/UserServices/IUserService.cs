using UserManagementApiReverb.BusinessLayer.DTOs.User;
using UserManagementApiReverb.BusinessLayer.DTOs;

namespace UserManagementApiReverb.BusinessLayer.UserServices;

public interface IUserService
{
    Task<UserResponse> GetUserAsyncById(Guid UserId);
    Task<UserResponse> GetUserAsyncByEmail(string Email);
    Task<UserResponse> GetUserAsyncByUsername(string UserName);
    Task<UserResponse> CreateUserAsync(UserRequestRegister req);
    Task<UserResponse> UpdateUserAsync(UserRequestUpdate req);
    Task<bool> DeleteUserAsync(Guid UserId);
    Task<UserResponse> GetUserByEmailOrUsernameAsync(string? Email, string? Username);
    Task<PagedResult<UserResponse>> GetAllUsersPaginationAsync(Paging paging, Sorting sorting);

}