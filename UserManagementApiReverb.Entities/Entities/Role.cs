namespace UserManagementApiReverb.Entities.Entities;

public class Role
{
    public Guid RoleId { get; set; }
    public string RoleName { get; set; } = string.Empty;
    public string? RoleDescription { get; set; }
    public bool IsSystemRole { get; set; } = false;
    
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    
    // neden eşitleme yaptığımızı açıkladık User kısmında bu ifade için oradan bakabilirsin.
    public ICollection<UserRole> UserRoles { get; set; } = new List<UserRole>();
}