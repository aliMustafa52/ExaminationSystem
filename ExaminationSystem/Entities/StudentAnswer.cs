namespace ExaminationSystem.Entities
{
    public class StudentAnswer : BaseModel
    {
        public int StudentExamId { get; set; }
        public int QuestionId { get; set; }
        public int ChoiceId { get; set; }
        public bool IsCorrect { get; set; }


        public StudentExam StudentExam { get; set; } = default!;
        public Question Question { get; set; } = default!;
        public Choice Choice { get; set; } = default!;
    }
}
