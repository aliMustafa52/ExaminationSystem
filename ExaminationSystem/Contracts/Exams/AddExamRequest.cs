using ExaminationSystem.Entities.Enums;

namespace ExaminationSystem.Contracts.Exams
{
    public record AddExamRequest
    (
        string Name,
        string Description,
        ExamType ExamType,
        double Duration,
        int NumberOfQuestions
    );
}
