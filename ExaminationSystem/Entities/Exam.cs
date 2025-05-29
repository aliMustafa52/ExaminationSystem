using ExaminationSystem.Entities.Enums;

namespace ExaminationSystem.Entities
{
    public class Exam : BaseModel
    {
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public ExamType ExamType { get; set; }
        public double Duration { get; set; }

        public int CourseId { get; set; }
        public int InstructorId { get; set; }

        public Course Course { get; set; } = default!;
        public Instructor Instructor { get; set; } = default!;
        public ICollection<ExamQuestion> ExamQuestions { get; set; } = [];
        public ICollection<StudentExam> StudentExams { get; set; } = [];
    }
}
