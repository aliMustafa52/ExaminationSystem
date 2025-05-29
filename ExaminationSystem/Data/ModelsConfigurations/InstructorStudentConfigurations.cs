using ExaminationSystem.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ExaminationSystem.Data.ModelsConfigurations
{
    public class InstructorStudentConfigurations : IEntityTypeConfiguration<InstructorStudent>
    {
        public void Configure(EntityTypeBuilder<InstructorStudent> builder)
        {
            builder
                .HasIndex(e => new { e.InstructorId, e.StudentId })
                .IsUnique();

            builder
                .HasOne(s => s.Student)
                .WithMany(s => s.InstructorStudents)
                .HasForeignKey(s => s.StudentId)
                .OnDelete(DeleteBehavior.Restrict);

            builder
                .HasOne(s => s.Instructor)
                .WithMany(s => s.InstructorStudents)
                .HasForeignKey(s => s.InstructorId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
