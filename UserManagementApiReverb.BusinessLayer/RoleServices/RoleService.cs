using UserManagementApiReverb.BusinessLayer.DTOs.Role;
using UserManagementApiReverb.BusinessLayer.Mappings;
using UserManagementApiReverb.DataAccessLayer;

namespace UserManagementApiReverb.BusinessLayer.RoleServices;

public class RoleService : IRoleService
{
    
    private readonly AppDbContext _db;
    private readonly IRoleMapper _mapper;
    private readonly IUserMapper _userMapper;
    public RoleService(IRoleMapper mapper,  AppDbContext db)
    {
        _db = db;
        _mapper = mapper;
    }


    public Task<RoleResponse> GetRoleByRoleIdAsync(Guid roleId)
    {
        throw new NotImplementedException();
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