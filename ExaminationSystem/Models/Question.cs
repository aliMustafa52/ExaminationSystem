using ExaminationSystem.Models.Enums;

namespace ExaminationSystem.Models
{
    public class Question : BaseModel
    {
        public string Text { get; set; } = string.Empty;
        public QuestionDifficulty DifficultyLevel { get; set; }

        public int InstructorId { get; set; }
        public ICollection<Choice> Choices { get; set; } = [];

        public ICollection<ExamQuestion> ExamQuestions { get; set; } = [];
        public ICollection<StudentAnswer> Answers { get; set; } = [];
        public Instructor Instructor { get; set; } = default!;
    }
}
