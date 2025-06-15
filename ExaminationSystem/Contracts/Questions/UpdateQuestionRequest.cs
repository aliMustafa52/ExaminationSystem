using ExaminationSystem.Contracts.Choices;
using ExaminationSystem.Entities.Enums;

namespace ExaminationSystem.Contracts.Questions
{
    public record UpdateQuestionRequest
    (
        string Text,
        QuestionDifficulty DifficultyLevel,
        IEnumerable<UpdateChoiceRequest> Choices
    );
}
