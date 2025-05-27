namespace ExaminationSystem.Models
{
    public class Student : BaseModel
    {
        public ICollection<InstructorStudent> InstructorStudents { get; set; } = [];
        public ICollection<StudentCourse> StudentCourses { get; set; } = [];
        public ICollection<StudentExam> StudentExams { get; set; } = [];
    }
}
