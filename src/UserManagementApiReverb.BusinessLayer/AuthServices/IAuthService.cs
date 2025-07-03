using UserManagementApiReverb.BusinessLayer.DTOs.Auth;

namespace UserManagementApiReverb.BusinessLayer.AuthServices;

public interface IAuthService
{
    Task<LoginResponse> LoginUserAsync(LoginRequest loginRequest);
}