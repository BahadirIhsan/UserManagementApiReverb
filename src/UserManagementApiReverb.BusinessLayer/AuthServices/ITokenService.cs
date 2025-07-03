using UserManagementApiReverb.BusinessLayer.DTOs.Auth;
using UserManagementApiReverb.Entities.Entities;

namespace UserManagementApiReverb.BusinessLayer.AuthServices;

public interface ITokenService
{
    TokenResult GenerateToken(User user);
}