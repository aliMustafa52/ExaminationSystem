using ExaminationSystem.Contracts.Questions;
using ExaminationSystem.Entities.Enums;

namespace ExaminationSystem.Contracts.Exams
{
    public record ExamResponseWithQuestions
    (
        int Id,
        string Name,
        string Description,
        ExamType ExamType,
        double Duration,
        int NumberOfQuestions,
        int CourseId,
        int InstructorId,
        IEnumerable<QuestionInExamResponse> Questions
    );
}
