using ExaminationSystem.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ExaminationSystem.Data.ModelsConfigurations
{
    public class ExamConfigurations : IEntityTypeConfiguration<Exam>
    {
        public void Configure(EntityTypeBuilder<Exam> builder)
        {
            builder
                .HasOne(e => e.Instructor)
                .WithMany(i => i.Exams)
                .HasForeignKey(e => e.InstructorId)
                .OnDelete(DeleteBehavior.Restrict);

            builder
                .HasOne(e => e.Course)
                .WithMany(i => i.Exams)
                .HasForeignKey(e => e.CourseId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
