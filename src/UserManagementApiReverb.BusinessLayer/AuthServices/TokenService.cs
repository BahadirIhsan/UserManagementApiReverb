using System.Text;
using Microsoft.Extensions.Configuration;
using UserManagementApiReverb.BusinessLayer.DTOs.Auth;
using UserManagementApiReverb.Entities.Entities;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.IdentityModel.Tokens;
using System.Security.Claims;
using JwtRegisteredClaimNames = Microsoft.IdentityModel.JsonWebTokens.JwtRegisteredClaimNames;

namespace UserManagementApiReverb.BusinessLayer.AuthServices;

public class TokenService : ITokenService
{
    private readonly IConfiguration _con;

    public TokenService(IConfiguration con)
    {
        _con = con;
    }
    public TokenResult GenerateToken(User user)
    {
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_con["Jwt:Key"]));
        
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
        
        var expiresAt = DateTime.UtcNow.AddMinutes(double.Parse(_con["Jwt:ExpiresMinutes"]!));

        var claims = new List<Claim>
        {
            new(JwtRegisteredClaimNames.Sub, user.UserId.ToString()),
            new(JwtRegisteredClaimNames.Email, user.Email)
        };

        foreach (var role in user.UserRoles.Select(x => x.Role.RoleName))
        {
            claims.Add(new Claim(ClaimTypes.Role, role));
        }

        var token = new JwtSecurityToken(
            issuer: _con["Jwt:Issuer"],
            audience: _con["Jwt:Audience"],
            claims : claims,
            expires: expiresAt,
            signingCredentials: creds);
        
        var tokenStr = new JwtSecurityTokenHandler().WriteToken(token);

        return new TokenResult
        {
            AccessToken = tokenStr,
            ExpiresAt = expiresAt
        };
    }
}