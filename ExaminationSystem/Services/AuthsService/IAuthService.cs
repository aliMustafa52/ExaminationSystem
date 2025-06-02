using ExaminationSystem.Abstractions;
using ExaminationSystem.Contracts.Authentication;

namespace ExaminationSystem.Services.AuthsService
{
    public interface IAuthService
    {
        Task<Result<AuthResponse>> GetTokenAsync(string email, string password, CancellationToken cancellationToken = default);

        Task<Result<AuthResponse>> GetRefreshAsync(string token, string refreshToken, CancellationToken cancellationToken = default);

        Task<Result> RevokeRefreshAsync(string token, string refreshToken, CancellationToken cancellationToken = default);
    }
}
