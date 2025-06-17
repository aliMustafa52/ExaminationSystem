using ExaminationSystem.Entities.Enums;

namespace ExaminationSystem.Contracts.Exams
{
    public record AutoExamQuestionsRequest
    (
        int EasyQuestions,
        int MediumQuestions,
        int HardQuestions
    );
}
