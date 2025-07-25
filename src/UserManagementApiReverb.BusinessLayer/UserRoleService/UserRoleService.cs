using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using UserManagementApiReverb.BusinessLayer.DTOs.UserRole;
using UserManagementApiReverb.BusinessLayer.Logging;
using UserManagementApiReverb.BusinessLayer.Mappings;
using UserManagementApiReverb.DataAccessLayer.Repositories.Interfaces;

namespace UserManagementApiReverb.BusinessLayer.UserRoleService;

public class UserRoleService :  IUserRoleService
{
    private readonly IUserRoleRepository _repo;
    private readonly IUserRoleMapper  _mapper;
    private readonly IAppLogger _logger;

    public UserRoleService(IUserRoleRepository repo , IUserRoleMapper mapper,  IAppLogger logger)
    {
        _repo = repo;
        _mapper = mapper;
        _logger = logger;
    }
    
    public async Task AssignRoleAsync(UserRoleAssign req)
    {
        bool exists = await _repo.UserRoleExistsAsync(req.UserId, req.RoleId);

        if (exists)
        {
            _logger.LogWarn("User already has this role", LogCategories.Audit, new { req.UserId, req.RoleId });
            throw new InvalidOperationException("User already assigned to this role");
        }

        var userRole = _mapper.MapFromAssignRequest(req);
        await _repo.AddUserRoleAsync(userRole);

        _logger.LogInfo("Assigned Role to User", LogCategories.Audit, new { req.UserId, req.RoleId });
    }

    public async Task<bool> RemoveRoleAsync(UserRoleRemove req)
    {
        var userRoles = await _repo.GetUserRolesAsync(req.UserId);

        if (!userRoles.Any())
        {
            _logger.LogWarn("User does not have this role", LogCategories.Audit, new { req.UserId });
            throw new InvalidOperationException("User does not have any roles");
        }

        if (userRoles.Count == 1 && userRoles[0].RoleId == req.RoleId)
        {
            _logger.LogWarn("Attempt to remove only role of User", LogCategories.Audit, new { req.UserId, req.RoleId });
            throw new InvalidOperationException("Cannot remove the only role of an user");
        }

        var hedefRole = userRoles.FirstOrDefault(u => u.RoleId == req.RoleId);

        if (hedefRole == null)
        {
            _logger.LogWarn("User does not have role", LogCategories.Audit, new { req.UserId, req.RoleId });
            throw new InvalidOperationException("User does not have this role");
        }

        hedefRole.RemovedReason = req.RemovedReason;
        hedefRole.RemovedAt = DateTime.Now;

        await _repo.RemoveUserRoleAsync(hedefRole);

        _logger.LogInfo("Removed Role", LogCategories.Audit, new { req.UserId, req.RoleId });
        return true;
    }

    public async Task<List<UserRoleResponse>> GetRolesByUserIdAsync(Guid userId)
    {
        var userRoles = await _repo.GetRolesByUserIdAsync(userId);
        _logger.LogInfo("Getting Roles for User", LogCategories.Audit, new { userId });
        return userRoles.Select(_mapper.MapUserRoleToResponse).ToList();
    }

    public async Task<List<RoleUserResponse>> GetUsersByRoleIdAsync(Guid roleId)
    {
        var roleUsers = await _repo.GetUsersByRoleIdAsync(roleId);
        _logger.LogInfo("Getting Users for Role", LogCategories.Audit, new { roleId });
        return roleUsers.Select(_mapper.MapRoleUserToResponse).ToList();
    }
}