using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using UserManagementApiReverb.Entities.Entities;

namespace UserManagementApiReverb.DataAccessLayer.Configurations;

public class UserConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.ToTable("Users"); // tablo adını belirlemek içim yazılan kısım.
        builder.HasKey(u => u.UserId);
        
        builder.Property(u => u.UserId)
            .ValueGeneratedOnAdd(); // guidleri ef ve db üretsin diye yapılan bir işlem client tarafının uğraşmasını istemiyor.
        
        builder.Property(u => u.UserName)
            .IsRequired()
            .HasMaxLength(50);
        
        builder.Property(u => u.Email)
            .IsRequired()
            .HasMaxLength(255);
        
        builder.Property(u => u.PasswordHash)
            .IsRequired()
            .HasMaxLength(255);
        
        builder.Property(u => u.FirstName)
            .IsRequired()
            .HasMaxLength(50);
        
        builder.Property(u => u.LastName)
            .IsRequired()
            .HasMaxLength(50);
        
        builder.Property(u => u.PhoneNumber)
            .HasMaxLength(11);
        
        builder.Property(u => u.Address)
            .HasMaxLength(300);
        
        builder.Property(u => u.Birthday)
            .HasColumnType("date");
        
        builder.Property(u => u.CreatedAt)
            .HasDefaultValueSql("GETUTCDATE()"); // sunucu saat farklarını engelleyip otomatik olarak zaman damgası verir.
        
        builder.HasIndex(u => u.UserName).IsUnique();
        builder.HasIndex(u => u.Email).IsUnique();

        // UserRole tablosuyla ilişki kurulur burada. FK oluşturulur.
        builder.HasMany(u => u.UserRoles)
            .WithOne(u => u.User)
            .HasForeignKey(u => u.UserId);
        
        
        //--- (İsteğe Bağlı) SOFT DELETE + CONCURRENCY -----------------------
        // Eğer entity'ye IsDeleted ve RowVersion eklediysen:
        // b.Property(u => u.IsDeleted).HasDefaultValue(false);
        // b.HasIndex(u => u.IsDeleted)
        //  .HasFilter("[IsDeleted] = 0");
        //
        // b.Property(u => u.RowVersion).IsRowVersion();

    }
}