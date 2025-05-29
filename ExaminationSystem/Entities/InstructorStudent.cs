namespace ExaminationSystem.Entities
{
    public class InstructorStudent : BaseModel
    {
        public int StudentId { get; set; }
        public Student Student { get; set; } = default!;

        public int InstructorId { get; set; }
        public Instructor Instructor { get; set; } = default!;
    }
}
