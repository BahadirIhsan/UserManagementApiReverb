using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using UserManagementApiReverb.BusinessLayer.DTOs;
using UserManagementApiReverb.BusinessLayer.DTOs.User;
using UserManagementApiReverb.BusinessLayer.UserServices;

namespace UserManagementApiReverb.PresentationLayer.Controllers;

[ApiController]
[Route("api/Users")]
public class UserController:  ControllerBase
{
    private readonly IUserService _userService;
    
    public UserController(IUserService userService)
    {
        _userService = userService;
    }

    [HttpGet("{UserId:Guid}")]
    public async Task<ActionResult<UserResponse>> GetById([FromQuery] Guid UserId)
    {
        var user = await _userService.GetUserAsyncById(UserId);
        if (user == null)
        {
            return NotFound();
        }
        return Ok(user);
    }

    [HttpGet("email/{Email}")]
    public async Task<ActionResult<UserResponse>> GetByEmail(string Email)
    {
        var user = await _userService.GetUserAsyncByEmail(Email);
        if (user == null)
        {
            return NotFound();
        }
        return Ok(user);
    }

    [HttpGet("UserName/{UserName}")]
    public async Task<ActionResult<UserResponse>> GetByUserName(string UserName)
    {
        var user = await _userService.GetUserAsyncByUsername(UserName);
        if (user == null)
        {
            return NotFound();
        }
        return Ok(user);
    }

    [HttpGet("ByEmailOrUserName")]
    public async Task<ActionResult<UserResponse>> GetByEmailOrUserName(string? Email, string? UserName)
    {
        var user =  await _userService.GetUserByEmailOrUsernameAsync(Email, UserName);
        if (user == null)
        {
            return NotFound();
        }
        return Ok(user);
    }

    [Authorize(Roles = "Admin")]
    [HttpGet("PaginationAllUsers")]
    public async Task<ActionResult<PagedResult<UserResponse>>> GetAllUsersWithPagination([FromQuery] Paging paging, [FromQuery] Sorting sorting)
    {
        if (paging.Page <= 0 || paging.PageSize <= 0)
        {
            return BadRequest();
        }
        
        var users = await _userService.GetAllUsersPaginationAsync(paging, sorting);
        
        Response.Headers.Add("X-Total-Count", users.TotalCount.ToString());
        
        return Ok(users);
    }
    /// <summary>
    /// Registers a new user to the system.
    /// </summary>
    [HttpPost("SignUp")]
    [ProducesResponseType(typeof(UserResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(UserResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(UserResponse), StatusCodes.Status409Conflict)]
    public async Task<ActionResult<UserResponse>> Create([FromBody] UserRequestRegister req)
    {
        try
        {
            var user = await _userService.CreateUserAsync(req);
            if (user == null)
            {
                return BadRequest();
            }
            return CreatedAtAction(nameof(GetById), new {UserId =  user.UserId}, user);
        }
        catch (ArgumentException e)
        {
            return Conflict(e.Message);
        }
        
    }

    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(UserResponse), StatusCodes.Status200OK)]
    [HttpPut("UpdateUser")]
    public async Task<ActionResult<UserResponse>> Update(Guid UserId, UserRequestUpdate req)
    {
        if (req.Id == Guid.Empty || req.Id != UserId)
        {
            return BadRequest(new ErrorResponse
            {
                StatusCode = 400,
                Message = "Invalid request"
            });
        }
        req.Id = UserId;
        var  user = await _userService.UpdateUserAsync(req);

        if (user == null)
        {
            return BadRequest(new ErrorResponse
            {
                StatusCode = 400,
                Message = "User not found"
            });
        }
        return Ok(user);
    }

    [HttpDelete("Delete")]
    public async Task<ActionResult<UserResponse>> Delete(Guid UserId)
    {
        var user = await _userService.DeleteUserAsync(UserId);
        
        if (!user)
        {
            return NotFound();
        }
        
        return NoContent();
    }
    
}