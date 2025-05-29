using ExaminationSystem.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ExaminationSystem.Data.ModelsConfigurations
{
    public class StudentAnswerConfigurations : IEntityTypeConfiguration<StudentAnswer>
    {
        public void Configure(EntityTypeBuilder<StudentAnswer> builder)
        {
            builder
                .HasOne(x => x.StudentExam)
                .WithMany(s => s.Answers)
                .HasForeignKey(x => x.StudentExamId)
                .OnDelete(DeleteBehavior.Restrict);

            builder
                .HasOne(x => x.Question)
                .WithMany(s => s.Answers)
                .HasForeignKey(x => x.QuestionId)
                .OnDelete(DeleteBehavior.Restrict);

            builder
                .HasOne(x => x.Choice)
                .WithMany(s => s.Answers)
                .HasForeignKey(x => x.ChoiceId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
