using System.Text;
using Microsoft.Extensions.Configuration;
using UserManagementApiReverb.BusinessLayer.DTOs.Auth;
using UserManagementApiReverb.Entities.Entities;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.IdentityModel.Tokens;
using System.Security.Claims;
using JwtRegisteredClaimNames = Microsoft.IdentityModel.JsonWebTokens.JwtRegisteredClaimNames;
using UserManagementApiReverb.BusinessLayer.Logging;

namespace UserManagementApiReverb.BusinessLayer.AuthServices;

public class TokenService : ITokenService
{
    private readonly IConfiguration _con;
    private readonly IAppLogger _logger;

    public TokenService(IConfiguration con, IAppLogger logger)
    {
        _con = con;
        _logger = logger;
    }

    public TokenResult GenerateToken(User user)
    {
        try
        {
            // Null kullanıcı kontrolü
            if (user == null || user.UserId == Guid.Empty)
            {
                _logger.LogWarn("Token generation failed: Invalid user object", LogCategories.Security, null);
                throw new ArgumentNullException(nameof(user));
            }

            // Config doğrulama
            var jwtKey = _con["Jwt:Key"];
            var jwtIssuer = _con["Jwt:Issuer"];
            var jwtAudience = _con["Jwt:Audience"];
            var jwtExpires = _con["Jwt:ExpiresMinutes"];

            if (string.IsNullOrWhiteSpace(jwtKey) || string.IsNullOrWhiteSpace(jwtIssuer) || string.IsNullOrWhiteSpace(jwtAudience))
            {
                _logger.LogError("JWT configuration missing", new ApplicationException("Config missing"),
                    LogCategories.Security, new { jwtKey, jwtIssuer, jwtAudience });
                throw new ApplicationException("JWT configuration is missing");
            }

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var expiresAt = DateTime.UtcNow.AddMinutes(double.Parse(jwtExpires!));

            var claims = new List<Claim>
            {
                new(JwtRegisteredClaimNames.Sub, user.UserId.ToString()),
                new(JwtRegisteredClaimNames.Email, user.Email ?? string.Empty)
            };

            foreach (var role in user.UserRoles.Select(x => x.Role.RoleName))
            {
                claims.Add(new Claim(ClaimTypes.Role, role));
            }

            var token = new JwtSecurityToken(
                issuer: jwtIssuer,
                audience: jwtAudience,
                claims: claims,
                expires: expiresAt,
                signingCredentials: creds);

            var tokenStr = new JwtSecurityTokenHandler().WriteToken(token);

            _logger.LogInfo("JWT token generated successfully", LogCategories.Security,
                new { user.UserId, user.Email, expiresAt });

            return new TokenResult
            {
                AccessToken = tokenStr,
                ExpiresAt = expiresAt
            };
        }
        catch (Exception ex)
        {
            _logger.LogError("Failed to generate JWT token", ex, LogCategories.Security,
                new { user?.UserId, user?.Email });

            throw new ApplicationException("Token generation failed");
        }
    }
}
