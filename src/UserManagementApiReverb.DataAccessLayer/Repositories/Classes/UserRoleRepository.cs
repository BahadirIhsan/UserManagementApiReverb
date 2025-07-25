using Microsoft.EntityFrameworkCore;
using UserManagementApiReverb.DataAccessLayer.Repositories.Interfaces;
using UserManagementApiReverb.Entities.Entities;

namespace UserManagementApiReverb.DataAccessLayer.Repositories.Classes;

public class UserRoleRepository : IUserRoleRepository
{
    private readonly AppDbContext _context;

    public UserRoleRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<bool> UserRoleExistsAsync(Guid userId, Guid roleId)
    {
        return await _context.UserRoles.AnyAsync(u => u.UserId == userId && u.RoleId == roleId);
    }

    public async Task AddUserRoleAsync(UserRole userRole)
    {
        await _context.UserRoles.AddAsync(userRole);
        await _context.SaveChangesAsync();
    }

    public async Task RemoveUserRoleAsync(UserRole userRole)
    {
        _context.UserRoles.Remove(userRole);
        await _context.SaveChangesAsync();
    }

    public async Task<List<UserRole>> GetRolesByUserIdAsync(Guid userId)
    {
        return await _context.UserRoles.AsNoTracking().Include(u => u.Role)
            .Where(u => u.UserId == userId).ToListAsync();
    }

    public async Task<List<UserRole>> GetUsersByRoleIdAsync(Guid roleId)
    {
        return await _context.UserRoles.AsNoTracking().Include(u => u.User)
            .Where(u => u.RoleId == roleId).ToListAsync();
    }

    public async Task<List<UserRole>> GetUserRolesAsync(Guid userId)
    {
        return await _context.UserRoles.Where(u => u.UserId == userId).ToListAsync();
    }
}