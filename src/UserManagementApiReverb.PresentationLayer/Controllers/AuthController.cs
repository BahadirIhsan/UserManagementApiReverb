using Microsoft.AspNetCore.Mvc;
using UserManagementApiReverb.BusinessLayer.AuthServices;
using UserManagementApiReverb.BusinessLayer.DTOs.Auth;

namespace UserManagementApiReverb.PresentationLayer.Controllers;

[ApiController]
[Route("api/Auth")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _auth;
    public AuthController(IAuthService auth)
    {
        _auth = auth;
    }

    [HttpPost("login")]
    public async Task<ActionResult> Login([FromBody] LoginRequest req)
    {
        try
        {
            var res = await _auth.LoginUserAsync(req);

            if (res == null)
            {
                return Unauthorized("Invalid username or password");
            }

            return Ok(res);
        }
        catch (UnauthorizedAccessException e)
        {
            return Unauthorized(e.Message);
        }
        catch (Exception e)
        {
            return StatusCode(500 ,"Internal Server Error");
        }
    }
}