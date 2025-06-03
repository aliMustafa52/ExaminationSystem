namespace ExaminationSystem.Contracts.Authentication
{
    public record StudentRegisterRequest
    (
        string Email,
        string Password,
        string FirstName,
        string LastName,
        string Address
    );
}
