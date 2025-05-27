namespace ExaminationSystem.Models
{
    public class Course : BaseModel
    {
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public int Hours { get; set; }
        public int InstructorId { get; set; }

        public ICollection<Exam> Exams { get; set; } = [];
        public ICollection<StudentCourse> StudentCourses { get; set; } = [];
        public Instructor Instructor { get; set; } = default!;
    }
}
