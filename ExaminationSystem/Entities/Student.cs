namespace ExaminationSystem.Entities
{
    public class Student : BaseModel
    {
        public string Address { get; set; } = string.Empty;
        public ICollection<InstructorStudent> InstructorStudents { get; set; } = [];
        public ICollection<StudentCourse> StudentCourses { get; set; } = [];
        public ICollection<StudentExam> StudentExams { get; set; } = [];

        public string AppUserId { get; set; } = string.Empty;
        public AppUser AppUser { get; set; } = default!;
    }
}
