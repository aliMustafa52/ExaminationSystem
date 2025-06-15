namespace ExaminationSystem.Contracts.Exams
{
    public record UpdateExamRequest
    (
        string Name,
        string Description,
        double Duration
    );
}
