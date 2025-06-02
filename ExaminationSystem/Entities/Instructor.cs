namespace ExaminationSystem.Entities
{
    public class Instructor : BaseModel
    {
        public string Name { get; set; } = string.Empty;
        public int Age { get; set; }
        public ICollection<InstructorStudent> InstructorStudents { get; set; } = [];
        public ICollection<Course> Courses { get; set; } = [];

        public ICollection<Exam> Exams { get; set; } = [];
        public ICollection<Question> Questions { get; set; } = [];
    }
}
