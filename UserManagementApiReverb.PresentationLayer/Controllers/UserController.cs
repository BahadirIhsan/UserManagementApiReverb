using Microsoft.AspNetCore.Mvc;
using UserManagementApiReverb.BusinessLayer.DTOs.User;
using UserManagementApiReverb.BusinessLayer.UserServices;

namespace UserManagementApiReverb.PresentationLayer.Controllers;

[ApiController]
[Route("api/users")]
public class UserController:  ControllerBase
{
    private readonly IUserService _userService;
    
    public UserController(IUserService userService)
    {
        _userService = userService;
    }

    [HttpGet("{UserId:Guid}")]
    public async Task<ActionResult<UserResponse>> GetById(Guid UserId)
    {
        var user = await _userService.GetUserAsyncById(UserId);
        if (user == null)
        {
            return NotFound();
        }
        return Ok(user);
    }

    [HttpGet("{Email}")]
    public async Task<ActionResult<UserResponse>> GetByEmail(string Email)
    {
        var user = await _userService.GetUserAsyncByEmail(Email);
        if (user == null)
        {
            return NotFound();
        }
        return Ok(user);
    }

    [HttpGet("{UserName}")]
    public async Task<ActionResult<UserResponse>> GetByUserName(string UserName)
    {
        var user = await _userService.GetUserAsyncByUsername(UserName);
        if (user == null)
        {
            return NotFound();
        }
        return Ok(user);
    }
}