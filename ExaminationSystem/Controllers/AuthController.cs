using ExaminationSystem.Abstractions;
using ExaminationSystem.Contracts.Authentication;
using ExaminationSystem.Services.AuthsService;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ExaminationSystem.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class AuthController(IAuthService authService) : ControllerBase
    {
        private readonly IAuthService _authService = authService;

        [HttpPost("")]
        public async Task<IActionResult> Login(LoginRequest request, CancellationToken cancellationToken)
        {
            var result = await _authService.GetTokenAsync(request.Email, request.Password, cancellationToken);
            return result.IsSuccess
                ? Ok(result.Value)
                : result.ToProblem();
        }

        [HttpPost("refresh")]
        public async Task<IActionResult> Refresh(RefreshTokenRequest request, CancellationToken cancellationToken)
        {
            var result = await _authService.GetRefreshAsync(request.Token, request.RefreshToken, cancellationToken);
            return result.IsSuccess
                ? Ok(result.Value)
                : result.ToProblem();
        }

        [HttpPost("revoke-refresh-token")]
        public async Task<IActionResult> Revoke(RefreshTokenRequest request, CancellationToken cancellationToken)
        {
            var result = await _authService.RevokeRefreshAsync(request.Token, request.RefreshToken, cancellationToken);
            return result.IsSuccess
                ? Ok()
                : result.ToProblem();
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register(RegisterRequest request, CancellationToken cancellationToken)
        {
            var result = await _authService.RegisterAsync(request, cancellationToken);
            return result.IsSuccess
                ? Ok()
                : result.ToProblem();
        }

        [HttpPost("instructor-register")]
        public async Task<IActionResult> RegisterAsInstructor(InstructorRegisterRequest request, CancellationToken cancellationToken)
        {
            var result = await _authService.RegisterAsInstructorAsync(request, cancellationToken);
            return result.IsSuccess
                ? Ok()
                : result.ToProblem();
        }

        [HttpPost("student-register")]
        public async Task<IActionResult> RegisterAsInstructor(StudentRegisterRequest request, CancellationToken cancellationToken)
        {
            var result = await _authService.RegisterAsStudentAsync(request, cancellationToken);
            return result.IsSuccess
                ? Ok()
                : result.ToProblem();
        }
    }
}
