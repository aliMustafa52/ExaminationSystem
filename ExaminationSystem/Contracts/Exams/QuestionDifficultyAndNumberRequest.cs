using ExaminationSystem.Entities.Enums;

namespace ExaminationSystem.Contracts.Exams
{
    public record QuestionDifficultyAndNumberRequest
    (
        QuestionDifficulty QuestionDifficulty,
        int Number
    );
}
