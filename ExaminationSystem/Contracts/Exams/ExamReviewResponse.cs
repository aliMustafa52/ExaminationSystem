namespace ExaminationSystem.Contracts.Exams
{
    public record ExamReviewResponse
    (
        int ExamId,
        string ExamTitle,
        double Score,
        List<ReviewedQuestionResponse> Questions
    );
}
