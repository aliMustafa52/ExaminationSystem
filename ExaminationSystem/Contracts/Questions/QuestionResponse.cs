using ExaminationSystem.Contracts.Choices;
using ExaminationSystem.Entities.Enums;

namespace ExaminationSystem.Contracts.Questions
{
    public record QuestionResponse
    (
        int Id,
        string Text,
        QuestionDifficulty DifficultyLevel,
        IEnumerable<ChoiceResponse> Choices
    );
}
