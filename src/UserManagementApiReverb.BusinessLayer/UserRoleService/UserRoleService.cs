using Microsoft.EntityFrameworkCore;
using UserManagementApiReverb.BusinessLayer.DTOs.UserRole;
using UserManagementApiReverb.BusinessLayer.Mappings;
using UserManagementApiReverb.DataAccessLayer;

namespace UserManagementApiReverb.BusinessLayer.UserRoleService;

public class UserRoleService :  IUserRoleService
{
    private readonly AppDbContext _db;
    private readonly IUserRoleMapper  _mapper;

    public UserRoleService(AppDbContext db, IUserRoleMapper mapper)
    {
        _db = db;
        _mapper = mapper;
    }
    
    public async Task AssignRoleAsync(UserRoleAssign req)
    {
        bool exists = await _db.UserRoles.AnyAsync(u => u.UserId == req.UserId  && u.RoleId == req.RoleId);

        if (exists)
        {
            throw new InvalidOperationException("User already assigned to this role");
        }

        var userRole = _mapper.MapFromAssignRequest(req);
        _db.UserRoles.Add(userRole);
        await _db.SaveChangesAsync();
        
    }

    public async Task<bool> RemoveRoleAsync(UserRoleRemove req)
    {
        var userRole = await _db.UserRoles.Where(u => u.UserId == req.UserId).ToListAsync();

        if (!userRole.Any())
        {
            throw new InvalidOperationException("User does not have any roles");
        }

        if (userRole.Count == 1 && userRole[0].RoleId == req.RoleId)
        {
            throw new InvalidOperationException("Cannot remove the only role of an user");
        }
        
        var hedefRole = userRole.FirstOrDefault(u => u.RoleId == req.RoleId);

        if (hedefRole == null)
        {
            throw new InvalidOperationException("User does not have this role");
        }
        
        hedefRole.RemovedReason = req.RemovedReason;
        hedefRole.RemovedAt = DateTime.Now;
        
        _db.UserRoles.Remove(hedefRole);
        await _db.SaveChangesAsync();
        return true;
        
    }

    public async Task<List<UserRoleResponse>> GetRolesByUserIdAsync(Guid userId)
    {

        var userRoles = await _db.UserRoles.AsNoTracking().Include(u => u.Role)
            .Where(u => u.UserId == userId).ToListAsync();
        
        var response = userRoles.Select(_mapper.MapUserRoleToResponse).ToList();
        
        return response;
    }

    public async Task<List<RoleUserResponse>> GetUsersByRoleIdAsync(Guid roleId)
    {
        var roleUsers = await _db.UserRoles.AsNoTracking().Include(u => u.User)
            .Where(u => u.RoleId == roleId).ToListAsync();

        var response = roleUsers.Select(_mapper.MapRoleUserToResponse).ToList();
        
        return response;
    }
}