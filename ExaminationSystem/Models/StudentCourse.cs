namespace ExaminationSystem.Models
{
    public class StudentCourse : BaseModel
    {
        public int StudentId { get; set; }
        public Student Student { get; set; } = default!;

        public int CourseId { get; set; }
        public Course Course { get; set; } = default!;
    }
}
