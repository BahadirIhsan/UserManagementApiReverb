using Microsoft.EntityFrameworkCore;
using UserManagementApiReverb.BusinessLayer.DTOs.Role;
using UserManagementApiReverb.BusinessLayer.Mappings;
using UserManagementApiReverb.DataAccessLayer;

namespace UserManagementApiReverb.BusinessLayer.RoleServices;

public class RoleService : IRoleService
{
    
    private readonly AppDbContext _db;
    private readonly IRoleMapper _mapper;
    public RoleService(IRoleMapper mapper,  AppDbContext db)
    {
        _db = db;
        _mapper = mapper;
    }


    public async Task<RoleResponse> GetRoleByRoleIdAsync(Guid roleId)
    {
        // var role = await _db.Roles.FindAsync(roleId); // burada primaryKey olduğundan dolayı FindAsync ile arama yaptım ve aynı zamanda şu an projem küçük 
        // bir proje olduğu için tracked olup olmaması beni ilgilendirmiyor eğer tracked olmasını istemiyorsak AsNoTracking ve devamında ise aramayı FirstOrDefault
        // ile yaparız. yukarıdaki işlemi yapmaya gerek yok çünkü bu kayıt vesaire yapıyor bunun yerine alttaki işlemi yaoarak kayıt tutmadan dah ahızlı ve memory'de
        // tutmadan rahat bir şekilde işlemleri yaparız.

        var role = await _db.Roles.AsNoTracking().FirstOrDefaultAsync(r => r.RoleId == roleId);
        
        if (role == null)
        {
            return null;
        }

        return _mapper.MapRoleToResponse(role);
    }

    public Task<RoleResponse> GetRoleByNameAsync(string roleName)
    {
        throw new NotImplementedException();
    }

    public Task<RoleResponse> CreateRoleAsync(RoleCreateRequest req)
    {
        throw new NotImplementedException();
    }

    public Task<RoleResponse> UpdateRoleAsync(Guid roleId, RoleUpdateRequest req)
    {
        throw new NotImplementedException();
    }

    public Task<bool> DeleteRoleAsync(Guid roleId)
    {
        throw new NotImplementedException();
    }

    public Task<IEnumerable<RoleWithUserCountResponse>> GetAllRolesAsync(bool? IsSystemRole = null)
    {
        throw new NotImplementedException();
    }
}