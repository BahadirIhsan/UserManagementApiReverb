namespace UserManagementApiReverb.Entities.Entities;

public class User
{
    public Guid UserId { get; set; }
    public string UserName { get; set; } = string.Empty;
    public string PasswordHash { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string? PhoneNumber { get; set; }
    public string? Address { get; set; }
    
    public DateTime CreatedAt { get; set; }
    public DateTime? Birthday { get; set; }
    
    // buradaki collection tanımlamasın da direk olarak collection'u initialize ediyoruz çünkü direkt olarak eleman ekleyebilmek için eğer başlatmasaydık
    // direk olarak .add ile eleman ekleyemezdik. null olarak başlatsaydık ise her ekleme öncesi if(var == null) gibi bir kontrol yapmaya ihtiyaç duyardık.
    public ICollection<UserRole> UserRoles { get; set; } =  new List<UserRole>();
}