namespace ExaminationSystem.Contracts.Choices
{
    public record ChoiceResponse
    (
        int Id,
        string Content,
        bool IsCorrect
    );
}
