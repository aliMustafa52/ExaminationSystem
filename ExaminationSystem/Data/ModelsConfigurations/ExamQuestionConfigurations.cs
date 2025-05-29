using ExaminationSystem.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ExaminationSystem.Data.ModelsConfigurations
{
    public class ExamQuestionConfigurations : IEntityTypeConfiguration<ExamQuestion>
    {
        public void Configure(EntityTypeBuilder<ExamQuestion> builder)
        {
            builder
                .HasIndex(e => new { e.ExamId, e.QuestionId })
                .IsUnique();

            builder
                .HasOne(e => e.Question)
                .WithMany(q => q.ExamQuestions)
                .HasForeignKey(e => e.QuestionId)
                .OnDelete(DeleteBehavior.Restrict);

            builder
                .HasOne(e => e.Exam)
                .WithMany(q => q.ExamQuestions)
                .HasForeignKey(e => e.ExamId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
