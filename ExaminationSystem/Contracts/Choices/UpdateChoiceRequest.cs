namespace ExaminationSystem.Contracts.Choices
{
    public record UpdateChoiceRequest
    (
        int Id,
        string Content,
        bool IsCorrect
    );
}
