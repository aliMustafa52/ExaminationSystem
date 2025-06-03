using ExaminationSystem.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ExaminationSystem.Data.ModelsConfigurations
{
    public class AppUserConfigurations : IEntityTypeConfiguration<AppUser>
    {
        public void Configure(EntityTypeBuilder<AppUser> builder)
        {
            builder
                .OwnsMany(x => x.RefreshTokens)
                .ToTable("RefreshTokens")
                .WithOwner()
                .HasForeignKey("UserId");

            builder
                .HasOne(u => u.Instructor)
                .WithOne(a => a.AppUser)
                .HasForeignKey<Instructor>(i => i.AppUserId)
                .OnDelete(DeleteBehavior.Cascade);

            builder
                .HasOne(u => u.Student)
                .WithOne(a => a.AppUser)
                .HasForeignKey<Student>(i => i.AppUserId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Property(x => x.FirstName)
                .HasMaxLength(100);

            builder.Property(x => x.LastName)
                .HasMaxLength(100);

        }
    }
}
