using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using UserManagementApiReverb.BusinessLayer.DTOs.UserRole;
using UserManagementApiReverb.BusinessLayer.UserRoleService;

namespace UserManagementApiReverb.PresentationLayer.Controllers;

[Authorize(Roles = "Admin")]
[ApiController]
[Route("Api/UserRole")]
public class UserRoleController : ControllerBase
{
    private readonly IUserRoleService _userRoleService;

    public UserRoleController(IUserRoleService userRoleService)
    {
        _userRoleService = userRoleService;
    }
    
    [HttpGet("UsersByRoleId")]
    public async Task<ActionResult<List<UserRoleResponse>>> GetUsersByRoleId(Guid roleId)
    {
        var users = await _userRoleService.GetUsersByRoleIdAsync(roleId);
        return Ok(users);
    }
    
    [HttpGet("RolesByUserId")]
    public async Task<ActionResult<List<RoleUserResponse>>> GetRolesByUserId(Guid UserId)
    {
        var roles = await _userRoleService.GetRolesByUserIdAsync(UserId);
        return Ok(roles);
    }
    
    [HttpPost("AssignRole")]
    public async Task<ActionResult> AssignRoleAsync(Guid userId, Guid roleId)
    {
        try
        {
            await _userRoleService.AssignRoleAsync(new UserRoleAssign
            {
                UserId = userId,
                RoleId = roleId
            });
            return Ok("Role assigned to user");
            
        }
        catch (InvalidOperationException e)
        {
            return BadRequest(e.Message);
        }
        
    }
    
    [HttpDelete("RemoveRoleFromUser")]
    public async Task<ActionResult> RemoveRoleFromUser(Guid userId, Guid roleId, [FromBody] string reason)
    {
        try
        {
            bool removedRole = await _userRoleService.RemoveRoleAsync(new UserRoleRemove
            {
                UserId = userId,
                RoleId = roleId,
                RemovedReason = reason
            });
            if (!removedRole)
            {
                return NotFound();
            }
            return NoContent();
        }
        catch (InvalidOperationException e)
        {
            return BadRequest(e.Message);
        }
    }
    
}