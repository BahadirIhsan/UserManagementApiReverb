using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using UserManagementApiReverb.BusinessLayer.DTOs;
using UserManagementApiReverb.BusinessLayer.DTOs.Role;
using UserManagementApiReverb.BusinessLayer.RoleServices;

namespace UserManagementApiReverb.PresentationLayer.Controllers;

[ApiController]
[Route("api/Roles")]
public class RoleController : ControllerBase
{
    private readonly IRoleService _roleService;

    public RoleController(IRoleService roleService)
    {
        _roleService = roleService;
    }

    [HttpGet("{roleId:Guid}")]
    public async Task<ActionResult<RoleResponse>> GetRoleById(Guid roleId)
    {
        var role = await _roleService.GetRoleByIdAsync(roleId);
        if (role == null)
        {
            return NotFound();
        }
        return Ok(role);
    }

    [HttpGet("roleName/{roleName}")]
    public async Task<ActionResult<RoleResponse>> GetRoleByNameAsync(string roleName)
    {
        var role = await _roleService.GetRoleByNameAsync(roleName);
        if (role == null)
        {
            return NotFound();
        }
        return Ok(role);
    }

    [HttpGet("PaginationAllRoles")]
    public async Task<ActionResult<PagedResult<RoleResponse>>> GetAllRolesWithPagination([FromQuery] Paging paging, [FromQuery] Sorting sorting)
    {
        if (paging.Page <= 0 || paging.PageSize <= 0)
        {
            return BadRequest();
        }
        
        var roles = await _roleService.GetAllRolesPaginationAsync(paging, sorting);
        Response.Headers.Add("X-Paging-Total-Count", roles.TotalCount.ToString()); // buradaki ifade de ToString() şeklinde yapmamız lazım
        // çünkü headers kısmındaki key value imzası otomatik olarak string type'dır method imzası bu şekilde olduğundan mecburi bir tanım bu.
        return Ok(roles);
    }
    
    [Authorize(Roles = "Admin, Manager")]
    [HttpPost("CreateRole")]
    public async Task<ActionResult<RoleResponse>> CreateRoleAsync(RoleCreateRequest req)
    {
        try
        {
            var role = await _roleService.CreateRoleAsync(req);
            return CreatedAtAction(nameof(GetRoleById), new {roleId = role.RoleId},  role);
        }
        catch (InvalidOperationException e)
        {
            return Conflict(e.Message);
        }
        
    }

    [Authorize(Roles = "Manager")]
    [HttpPut("UpdateRole")]
    public async Task<ActionResult<RoleResponse>> UpdateRoleAsync(Guid roleId, [FromBody] RoleUpdateRequest req)
    {
        try
        {
            if (roleId == Guid.Empty || roleId != req.Id)
            {
                return BadRequest("Id is invalid or empty");
            }
            req.Id = roleId;
            var role = await _roleService.UpdateRoleAsync(req);
            if (role == null)
            {
                return NotFound();
            }
            return Ok(role);
        }
        catch (InvalidOperationException e)
        {
            return BadRequest(e.Message);
        }
        
    }
    
    [Authorize(Roles = "Manager")]
    [HttpDelete("DeleteRole")]
    public async Task<ActionResult<RoleResponse>> DeleteRoleAsync(Guid roleId)
    {
        try
        {
            var role = await _roleService.DeleteRoleAsync(roleId);
            if (!role)
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