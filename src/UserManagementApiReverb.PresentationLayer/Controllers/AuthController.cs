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
        try
        {
            var res = await _auth.LoginUserAsync(req);

            if (res == null)
            {
                _logger.LogWarn("Failed login attempt: Invalid username or password", LogCategories.Security, new {req.Email});
                return Unauthorized("Invalid username or password");
            }

            _logger.LogInfo("User logged in successfully", LogCategories.Security, new {req.Email});
            return Ok(res);
        }
        catch (UnauthorizedAccessException e)
        {
            _logger.LogWarn("Controller Warning: Failed login attempt: user not found", LogCategories.Security, new {req.Email});
            return Unauthorized(e.Message);
        }
        catch (Exception e)
        {
            _logger.LogError("Controller Error: Unexpected error during login",e,  LogCategories.Security, new {req.Email});
            return StatusCode(500 ,"Internal Server Error");
        }
    }
}