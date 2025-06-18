namespace ExaminationSystem.Entities
{
    public class StudentExam : BaseModel
    {
        public int StudentId { get; set; }
        public int ExamId { get; set; }
        public double? Score { get; set; }

        public Student Student { get; set; } = default!;
        public Exam Exam { get; set; } = default!;

        public ICollection<StudentAnswer> Answers { get; set; } = [];
    }
}
