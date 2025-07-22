using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using UserManagementApiReverb.BusinessLayer.DTOs.UserRole;
using UserManagementApiReverb.BusinessLayer.Logging;
using UserManagementApiReverb.BusinessLayer.UserRoleService;

namespace UserManagementApiReverb.PresentationLayer.Controllers;

[Authorize(Roles = "Admin")]
[ApiController]
[Route("Api/UserRole")]
public class UserRoleController : ControllerBase
{
    private readonly IUserRoleService _userRoleService;
    private readonly IAppLogger _logger;

    public UserRoleController(IUserRoleService userRoleService,  IAppLogger logger)
    {
        _userRoleService = userRoleService;
        _logger = logger;
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
            _logger.LogInfo("Controller: Assigned role to user", LogCategories.Audit, new { userId, roleId });
            return Ok("Role assigned to user");

        }
        catch (InvalidOperationException e)
        {
            _logger.LogWarn("Controller Warning: Failed to assign role to user", LogCategories.Audit, new { userId, roleId });
            return BadRequest(e.Message);
        }
        catch (Exception e)
        {
            _logger.LogError("Controller Error: Unexpected error while assigning role to user",e ,LogCategories.Audit, new { userId, roleId });
            return StatusCode(500, "Unexpected error");
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
                _logger.LogWarn("Controller Warning: Tried to remove role from user, but role not found",
                    LogCategories.Audit, new { userId, roleId });
                return NotFound();
            }

            _logger.LogInfo("Controller: Removed role from user", LogCategories.Audit, new { userId, roleId });
            return NoContent();
        }
        catch (InvalidOperationException e)
        {
            _logger.LogWarn("Controller Warning: Failed to remove role from user", LogCategories.Audit,
                new { userId, roleId });
            return BadRequest(e.Message);
        }
        catch (Exception e)
        {
            _logger.LogError("Controller Error: Unexpected error while removing role from user",e ,LogCategories.Audit, new { userId, roleId });
            return StatusCode(500, "Unexpected error");
        }
    }
    
}