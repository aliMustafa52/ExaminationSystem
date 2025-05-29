using ExaminationSystem.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ExaminationSystem.Data.ModelsConfigurations
{
    public class StudentExamConfigurations : IEntityTypeConfiguration<StudentExam>
    {

        public void Configure(EntityTypeBuilder<StudentExam> builder)
        {
            builder
                .HasIndex(s => new { s.ExamId, s.StudentId })
                .IsUnique();

            builder
                .HasOne(s => s.Student)
                .WithMany(s => s.StudentExams)
                .HasForeignKey(s => s.StudentId)
                .OnDelete(DeleteBehavior.Restrict);

            builder
                .HasOne(s => s.Exam)
                .WithMany(s => s.StudentExams)
                .HasForeignKey(s => s.ExamId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
