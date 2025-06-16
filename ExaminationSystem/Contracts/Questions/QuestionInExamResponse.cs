using ExaminationSystem.Contracts.Choices;
using ExaminationSystem.Entities.Enums;

namespace ExaminationSystem.Contracts.Questions
{
    public record QuestionInExamResponse
    (
        int Id,
        string Text,
        IEnumerable<ChoiceInExamResponse> Choices
    );
}
