using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using UserManagementApiReverb.Entities.Entities;

namespace UserManagementApiReverb.DataAccessLayer.Configurations;

public class AuditLogConfiguration :  IEntityTypeConfiguration<AuditLog>
{
    public void Configure(EntityTypeBuilder<AuditLog> builder)
    {
        builder.ToTable("AuditLogs");
        builder.HasKey(x => x.Id);

        builder.Property(x => x.UserId).HasColumnType("char(36)");
        
        builder.Property(x => x.TableName).HasMaxLength(128).IsRequired();
        builder.Property(x => x.Action).HasMaxLength(128).IsRequired();
        
        builder.Property(a => a.OldValues).HasColumnType("longtext");
        builder.Property(a => a.NewValues).HasColumnType("longtext");
        /*
        builder.Property(x => x.OldValues)
            .HasColumnType("nvarchar(max)");
        
        builder.Property(x => x.NewValues)
            .HasColumnType("nvarchar(max)");
        */
        builder.HasOne(x => x.User)
            .WithMany()
            .HasForeignKey(x => x.UserId)
            .OnDelete(DeleteBehavior.SetNull); // burada yapılan işlem eğer olur da kullanıcı silinirse 
            // onun yapmış olduğu işlemler yani tutulan loglar silinmez onun yerine kullanıcının Id kısmı
            // null yapılır ama logları hala saklı bir şekilde db de kalır. UserId required yapılmışsa 
            // veya bu tarz durumlarda bu method çalışmaz ve hata alırız onun haricinde sorunsuz ve effective 
            // bir şekilde çalışır. 
            // "Cascade" kullanılırsa SetNull yerine loglarda silinir ama ben logların silinmesini istemediğim
            // için bu keyword'ü kullanmadım.

    }
}