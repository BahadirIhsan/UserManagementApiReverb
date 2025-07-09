using UserManagementApiReverb.Entities.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace UserManagementApiReverb.DataAccessLayer.Configurations;

public class RoleConfiguration : IEntityTypeConfiguration<Role>
{
    public void Configure(EntityTypeBuilder<Role> builder)
    {
        builder.ToTable("Roles");
        builder.HasKey(r => r.RoleId);
        
        builder.Property(r => r.RoleId)
            .ValueGeneratedOnAdd();
        
        builder.Property(r => r.RoleName)
            .IsRequired()
            .HasMaxLength(50);
        
        builder.Property(r => r.RoleDescription)
            .HasMaxLength(255);
    
        builder.Property(r => r.IsSystemRole)
            .HasDefaultValue(false);
        
        builder.Property(r => r.CreatedAt)
            .HasDefaultValueSql("GETUTCDATE()");
        
        // UpdatedAt’i burada kısıtlamıyoruz; uygulama müdahale edecek
        
        builder.HasIndex(r => r.RoleName).IsUnique();
        
        // Sistem rolleri üstünde sık filtre yapıyorsan:
        // b.HasIndex(r => r.IsSystemRole);
        
        builder.HasMany(r => r.UserRoles)
            .WithOne(r => r.Role)
            .HasForeignKey(r => r.RoleId);
        
        //--- (İsteğe Bağlı) SOFT DELETE & CONCURRENCY -----------------------
        // Eğer Role entity’sine IsDeleted, RowVersion ekleyeceksen
        // aynı User’da yaptığın Fluent ayarını burada da tekrarla.
        
    }
}