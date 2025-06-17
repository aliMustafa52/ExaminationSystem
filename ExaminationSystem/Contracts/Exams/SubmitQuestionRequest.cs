namespace ExaminationSystem.Contracts.Exams
{
    public record SubmitQuestionRequest
    (
        int QuestionId,
        int ChoiceId
    );
}
