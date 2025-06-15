using ExaminationSystem.Contracts.Choices;
using ExaminationSystem.Entities.Enums;

namespace ExaminationSystem.Contracts.Questions
{
    public record AddQuestionRequest
    (
        string Text,
        QuestionDifficulty DifficultyLevel,
        IEnumerable<AddChoiceRequest> Choices
    );
}
