namespace ExaminationSystem.Models
{
    public class Instructor : BaseModel
    {
        public ICollection<InstructorStudent> InstructorStudents { get; set; } = [];
        public ICollection<Course> Courses { get; set; } = [];

        public ICollection<Exam> Exams { get; set; } = [];
        public ICollection<Question> Questions { get; set; } = [];
    }
}
