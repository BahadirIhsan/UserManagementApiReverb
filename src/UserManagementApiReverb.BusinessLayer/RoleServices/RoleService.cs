using Microsoft.EntityFrameworkCore;
using UserManagementApiReverb.BusinessLayer.DTOs.Role;
using UserManagementApiReverb.BusinessLayer.Mappings;
using UserManagementApiReverb.DataAccessLayer;
using UserManagementApiReverb.Entities.Entities;

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

    public async Task<RoleResponse> GetRoleByNameAsync(string roleName)
    {
        var role = await _db.Roles.AsNoTracking().FirstOrDefaultAsync(r => r.RoleName == roleName);
        
        if (role == null)
        {
            return null;
        }
        return _mapper.MapRoleToResponse(role);
    }

    public async Task<RoleResponse> CreateRoleAsync(RoleCreateRequest req)
    {
        var newRole = _mapper.MapCreateRoleToResponse(req);
        _db.Roles.Add(newRole);
        await _db.SaveChangesAsync();
        return _mapper.MapRoleToResponse(newRole);
    }

    public async Task<RoleResponse> UpdateRoleAsync(Guid roleId, RoleUpdateRequest req)
    {
        var role = await _db.Roles.FindAsync(roleId);

        if (role == null)
        {
            return null;
        }

        if (role.IsSystemRole)
        {
            throw new InvalidOperationException("Cannot update system role");
        }
        
        role.RoleName = req.RoleName;
        role.RoleDescription = req.RoleDescription;
        role.UpdatedAt = DateTime.Now;

        await _db.SaveChangesAsync();
        //_db.Update(role); // bu satır gereksiz yere memory de fazlalık yapar çünkü bu tracked olmayan durumlarda özellikle bu elemanı değiştir gibi işlemlerde 
        // kullanılır buna ihtiyaç duymuyoruz çünkü bizim yapımız zaten bu blok için tracked olduğundan gerek duymuyoruz tracked olması sayesinde zaten otomatik olarak
        // değişimleri falan fark edip kendisi gerçekleştiriyor.
        return _mapper.MapRoleToResponse(role);
    }

    public async Task<bool> DeleteRoleAsync(Guid roleId)
    {
        var role = await _db.Roles.FindAsync(roleId);
        if (role == null)
        {
            return false;
        }

        if (role.IsSystemRole)
        {
            throw new InvalidOperationException("Cannot delete system role");
        }
        
        bool hasUsers = await _db.UserRoles.AnyAsync(ur => ur.RoleId == roleId);
        if (hasUsers)
        {
            throw new InvalidOperationException("Role is assigned to at least one user.");
        }
        _db.Roles.Remove(role);
        await _db.SaveChangesAsync();
        return true;
    }

    public async Task<IEnumerable<RoleWithUserCountResponse>> GetAllRolesAsync(bool? IsSystemRole = null)
    {
        // burada IQueryable bir tanım yapmamız lazım var tanımına göre var'a göre bir tanım yaparsak type uyuşmazlığı olur IIncludableQueryable<…>
        // bu şekilde algılar var bu nesneyi ama bu nesen aslına ikinci atama işleminde yani aşağıda where ile çağrıldığı zaman hatalı bir yapısı olur 
        // bu durumla karşılaşmamak için ve hatalı olacağı için en başta bu şekilde bir atama yaparız veya aşağıda typeCasting işlemiyle de bu işlemi 
        // yapabiliriz ama bu şekilde bir tanım yapmak daha efektif bir şekilde çalışır
        IQueryable<Role> query = _db.Roles.AsNoTracking().Include(r => r.UserRoles);

        if (IsSystemRole.HasValue)
        {
            query = query.Where(r => r.IsSystemRole == IsSystemRole.Value);
        }
        
        var roles = await query.ToListAsync();

        if (roles.Count == 0)
        {
            return Enumerable.Empty<RoleWithUserCountResponse>();
        }

        return roles.Select(r => _mapper.MapRoleWithUserCountToResponse(r));
    }
}