using Microsoft.AspNetCore.Mvc;
using UserManagementApiReverb.BusinessLayer.AuthServices;
using UserManagementApiReverb.BusinessLayer.DTOs.Auth;
using UserManagementApiReverb.BusinessLayer.Logging;

namespace UserManagementApiReverb.PresentationLayer.Controllers;

[ApiController]
[Route("api/Auth")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _auth;
    private readonly IAppLogger _logger;
    public AuthController(IAuthService auth, IAppLogger logger)
    {
        _auth = auth;
        _logger = logger;
    }

    [HttpPost("login")]
    public async Task<ActionResult> Login([FromBody] LoginRequest req)
    {
        var res = await _auth.LoginUserAsync(req);

        if (res == null)
        {
            _logger.LogWarn("Failed login attempt: Invalid password", LogCategories.Security, new { req.Email });
            return Unauthorized("Invalid username or password");
        }

        _logger.LogInfo("User logged in successfully", LogCategories.Security, new { req.Email });
        return Ok(res);
    }
}