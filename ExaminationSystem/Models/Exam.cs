namespace ExaminationSystem.Models
{
    public class Exam : BaseModel
    {
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;

        public int CourseId { get; set; }
        public Course Course { get; set; } = default!;
    }
}
