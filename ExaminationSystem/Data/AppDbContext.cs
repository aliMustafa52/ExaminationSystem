using ExaminationSystem.Entities;
using Microsoft.EntityFrameworkCore;
using System.Reflection;

namespace ExaminationSystem.Data
{
    public class AppDbContext(DbContextOptions<AppDbContext> options) 
        : DbContext(options)
    {
        public required DbSet<Choice> Answers { get; set; }
        public required DbSet<Course> Courses { get; set; }
        public required DbSet<Exam> Exams { get; set; }
        public required DbSet<ExamQuestion> ExamQuestions { get; set; }
        public required DbSet<Instructor> Instructors { get; set; }
        public required DbSet<InstructorStudent> InstructorStudents { get; set; }
        public required DbSet<Question> Questions { get; set; }
        public required DbSet<Student> Students { get; set; }
        public required DbSet<StudentCourse> StudentCourses { get; set; }
        public required DbSet<StudentExam> StudentExams { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking);

            base.OnConfiguring(optionsBuilder);
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());

            base.OnModelCreating(modelBuilder);
        }
    }
}
