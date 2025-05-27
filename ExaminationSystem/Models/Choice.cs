namespace ExaminationSystem.Models
{
    public class Choice : BaseModel
    {
        public string Content { get; set; } = string.Empty;
        public bool IsCorrect { get; set; }

        public int QuestionId { get; set; }
        public Question Question { get; set; } = default!;
        public ICollection<StudentAnswer> Answers { get; set; } = [];
    }
}
