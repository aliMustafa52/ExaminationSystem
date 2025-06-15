using ExaminationSystem.Entities.Enums;

namespace ExaminationSystem.Contracts.Exams
{
    public record ExamResponse
    (
        int Id,
        string Name,
        string Description,
        ExamType ExamType,
        double Duration,
        int NumberOfQuestions,
        int CourseId,
        string InstructorId
    );
}
