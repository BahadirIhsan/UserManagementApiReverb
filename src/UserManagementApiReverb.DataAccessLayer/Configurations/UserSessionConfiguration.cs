using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using UserManagementApiReverb.Entities.Entities;

namespace UserManagementApiReverb.DataAccessLayer.Configurations
{
    public class UserSessionConfiguration : IEntityTypeConfiguration<UserSession>
    {
        public void Configure(EntityTypeBuilder<UserSession> builder)
        {
            builder.ToTable("UserSessions");

            builder.HasKey(s => s.Id);

            builder.Property(s => s.AccessToken)
                .IsRequired()
                .HasColumnType("longtext");

            builder.Property(s => s.RefreshToken)
                .IsRequired()
                .HasColumnType("longtext");

            builder.Property(s => s.CreatedAt)
                .IsRequired();

            builder.Property(s => s.ExpiresAt)
                .IsRequired();

            builder.HasOne(s => s.User)
                .WithMany()
                .HasForeignKey(s => s.UserId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}