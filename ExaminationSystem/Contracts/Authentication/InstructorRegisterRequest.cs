namespace ExaminationSystem.Contracts.Authentication
{
    public record InstructorRegisterRequest
    (
        string Email,
        string Password,
        string FirstName,
        string LastName,
        int Age
    );
}
