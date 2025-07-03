using Microsoft.EntityFrameworkCore;
using UserManagementApiReverb.BusinessLayer.DTOs.Auth;
using UserManagementApiReverb.DataAccessLayer;

namespace UserManagementApiReverb.BusinessLayer.AuthServices;

public class AuthService : IAuthService
{
    private readonly AppDbContext _db;
    private readonly ITokenService _token;
    
    public AuthService(AppDbContext db, ITokenService token)
    {
        _db = db;
        _token = token;
    }
    
    public async Task<LoginResponse> LoginUserAsync(LoginRequest req)
    {
        var user = await _db.Users.Include(u => u.UserRoles)
                            .ThenInclude(u => u.Role)
                            .FirstOrDefaultAsync(u => u.Email == req.Email);

        if (user == null)
        {
            throw new UnauthorizedAccessException("User not found");
        }

        bool isValid = BCrypt.Net.BCrypt.Verify(req.Password, user.PasswordHash);
        if (!isValid)
        {
            return null;
        }
        
        var tokenResult = _token.GenerateToken(user);

        return new LoginResponse
        {
            AccessToken = tokenResult.AccessToken,
            ExpiresAt = tokenResult.ExpiresAt
        };
    }
}