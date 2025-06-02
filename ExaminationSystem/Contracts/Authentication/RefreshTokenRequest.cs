namespace ExaminationSystem.Contracts.Authentication
{
    public record RefreshTokenRequest
    (
        string Token,
        string RefreshToken
    );
}
