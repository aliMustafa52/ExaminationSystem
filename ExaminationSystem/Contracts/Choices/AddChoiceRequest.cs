namespace ExaminationSystem.Contracts.Choices
{
    public record AddChoiceRequest
    (
        string Content,
        bool IsCorrect
    );
}
