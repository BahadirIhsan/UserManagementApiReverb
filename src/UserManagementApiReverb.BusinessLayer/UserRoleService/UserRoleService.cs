using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using UserManagementApiReverb.BusinessLayer.DTOs.UserRole;
using UserManagementApiReverb.BusinessLayer.Logging;
using UserManagementApiReverb.BusinessLayer.Mappings;
using UserManagementApiReverb.DataAccessLayer;

namespace UserManagementApiReverb.BusinessLayer.UserRoleService;

public class UserRoleService :  IUserRoleService
{
    private readonly AppDbContext _db;
    private readonly IUserRoleMapper  _mapper;
    private readonly IAppLogger _logger;

    public UserRoleService(AppDbContext db, IUserRoleMapper mapper,  IAppLogger logger)
    {
        _db = db;
        _mapper = mapper;
        _logger = logger;
    }
    
    public async Task AssignRoleAsync(UserRoleAssign req)
    {
        bool exists = await _db.UserRoles.AnyAsync(u => u.UserId == req.UserId  && u.RoleId == req.RoleId);

        if (exists)
        {
            _logger.LogWarn("User already has this role", LogCategories.Audit, new { req.UserId, req.RoleId });
            throw new InvalidOperationException("User already assigned to this role");
        }

        var userRole = _mapper.MapFromAssignRequest(req);
        _db.UserRoles.Add(userRole);
        await _db.SaveChangesAsync();
        
        _logger.LogInfo("Assigned Role to User", LogCategories.Audit, new { req.UserId, req.RoleId });
        
    }

    public async Task<bool> RemoveRoleAsync(UserRoleRemove req)
    {
        var userRole = await _db.UserRoles.Where(u => u.UserId == req.UserId).ToListAsync();

        if (!userRole.Any())
        {
            _logger.LogWarn("User does not have this role", LogCategories.Audit, new { req.UserId });
            throw new InvalidOperationException("User does not have any roles");
        }

        if (userRole.Count == 1 && userRole[0].RoleId == req.RoleId)
        {
            _logger.LogWarn("Attempt to remove only role of User", LogCategories.Audit, new { req.UserId, req.RoleId });
            throw new InvalidOperationException("Cannot remove the only role of an user");
        }
        
        var hedefRole = userRole.FirstOrDefault(u => u.RoleId == req.RoleId);

        if (hedefRole == null)
        {
            _logger.LogWarn("User does not have role", LogCategories.Audit, new { req.UserId, req.RoleId });
            throw new InvalidOperationException("User does not have this role");
        }
        
        hedefRole.RemovedReason = req.RemovedReason;
        hedefRole.RemovedAt = DateTime.Now;
        
        _db.UserRoles.Remove(hedefRole);
        await _db.SaveChangesAsync();
        
        _logger.LogInfo("Removed Role", LogCategories.Audit, new { req.UserId, req.RoleId });
        return true;
        
    }

    public async Task<List<UserRoleResponse>> GetRolesByUserIdAsync(Guid userId)
    {

        var userRoles = await _db.UserRoles.AsNoTracking().Include(u => u.Role)
            .Where(u => u.UserId == userId).ToListAsync();
        
        _logger.LogInfo("Getting Roles for User", LogCategories.Audit, new { userId });
        
        var response = userRoles.Select(_mapper.MapUserRoleToResponse).ToList();
        
        return response;
    }

    public async Task<List<RoleUserResponse>> GetUsersByRoleIdAsync(Guid roleId)
    {
        var roleUsers = await _db.UserRoles.AsNoTracking().Include(u => u.User)
            .Where(u => u.RoleId == roleId).ToListAsync();

        _logger.LogInfo("Getting Roles for User", LogCategories.Audit, new { roleId });
        
        var response = roleUsers.Select(_mapper.MapRoleUserToResponse).ToList();
        
        return response;
    }
}