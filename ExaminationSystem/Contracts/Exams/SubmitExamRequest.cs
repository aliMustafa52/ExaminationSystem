namespace ExaminationSystem.Contracts.Exams
{
    public record SubmitExamRequest
    (
        IEnumerable<SubmitQuestionRequest> SubmitQuestions
    );
}
