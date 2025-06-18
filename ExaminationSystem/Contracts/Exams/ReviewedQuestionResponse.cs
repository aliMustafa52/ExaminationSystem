using ExaminationSystem.Contracts.Choices;

namespace ExaminationSystem.Contracts.Exams
{
    public record ReviewedQuestionResponse
    (
        int Id,
        string Text,
        int StudentChoiceId,
        int CorrectChoiceId,
        bool WasCorrect,
        List<ChoiceInExamResponse> Choices
    );
}
