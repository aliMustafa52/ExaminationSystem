namespace ExaminationSystem.Contracts.Exams
{
    public record ExamQuestionsRequest
    (
        IEnumerable<int> QuestionIds
    );
}
