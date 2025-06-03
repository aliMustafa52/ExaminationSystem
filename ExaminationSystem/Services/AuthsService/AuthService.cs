using ExaminationSystem.Abstractions;
using ExaminationSystem.Authentication;
using ExaminationSystem.Contracts.Authentication;
using ExaminationSystem.Entities;
using ExaminationSystem.Entities.Enums;
using ExaminationSystem.Errors;
using Mapster;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Data;
using System.Security.Cryptography;

namespace ExaminationSystem.Services.AuthsService
{
    public class AuthService(UserManager<AppUser> userManager, IJwtProvider jwtProvider) : IAuthService
    {
        private readonly UserManager<AppUser> _userManager = userManager;
        private readonly IJwtProvider _jwtProvider = jwtProvider;

        private readonly int _resfreshTokenExpiryDays = 30;

        public async Task<Result<AuthResponse>> GetTokenAsync(string email, string password, CancellationToken cancellationToken = default)
        {
            var user = await _userManager.FindByEmailAsync(email);
            if(user is null)
                return Result.Failure<AuthResponse>(UserErrors.UserNotFound);

            var isPasswordCorrect = await _userManager.CheckPasswordAsync(user, password);
            if (!isPasswordCorrect)
                return Result.Failure<AuthResponse>(UserErrors.UserNotFound);

            // generate Token
            var (token, expiresIn) = _jwtProvider.GenerateToken(user);

            // Generate Refresh Token
            var resfreshToken = GenerateRefreshToken();
            var resfreshTokenExpiration = DateTime.UtcNow.AddDays(_resfreshTokenExpiryDays);

            user.RefreshTokens.Add(new RefreshToken
            {
                Token = resfreshToken,
                ExpiresOn = resfreshTokenExpiration
            });

            await _userManager.UpdateAsync(user);

            var authResponse = new AuthResponse(user.Id, user.Email, user.FirstName, user.LastName, token, expiresIn * 60, resfreshToken, resfreshTokenExpiration);

            return Result.Success(authResponse);
        }

        public async Task<Result<AuthResponse>> GetRefreshAsync(string token, string refreshToken, CancellationToken cancellationToken = default)
        {
            var userId = _jwtProvider.ValidateToken(token);
            if (userId is null)
                return Result.Failure<AuthResponse>(UserErrors.UserInvalidAccessToken);

            var user = await _userManager.FindByIdAsync(userId);
            if (user is null)
                return Result.Failure<AuthResponse>(UserErrors.UserNotFound);

            if (user.IsDisabled)
                return Result.Failure<AuthResponse>(UserErrors.DisabledUser);

            if (user.LockoutEnd > DateTime.UtcNow)
                return Result.Failure<AuthResponse>(UserErrors.LockedOutUser);

            var userRefreshToken = user.RefreshTokens.SingleOrDefault(x => x.Token == refreshToken && x.IsActive);
            if (userRefreshToken is null)
                return Result.Failure<AuthResponse>(UserErrors.UserInvalidResreshToken);

            userRefreshToken.RevokedOn = DateTime.UtcNow;

            // generate new access Token
            var (newToken, expiresIn) = _jwtProvider.GenerateToken(user);

            // Generate new Refresh Token
            var newRefreshTokne = GenerateRefreshToken();
            var refreshTokenExpiration = DateTime.UtcNow.AddDays(_resfreshTokenExpiryDays);

            user.RefreshTokens.Add(new RefreshToken
            {
                Token = newRefreshTokne,
                ExpiresOn = refreshTokenExpiration
            });

            await _userManager.UpdateAsync(user);

            return Result.Success(new AuthResponse(user.Id, user.Email, user.FirstName, user.LastName, newToken, expiresIn * 60, newRefreshTokne, refreshTokenExpiration));
        }

        public async Task<Result> RegisterAsync(RegisterRequest registerRequest, CancellationToken cancellationToken = default)
        {
            //check if email exists 
            var isEmailExists = await _userManager.Users.AnyAsync(u => u.Email == registerRequest.Email, cancellationToken);
            if (isEmailExists)
                return Result.Failure(UserErrors.UserDublicatedEmail);

            var user = registerRequest.Adapt<AppUser>();

            var result = await _userManager.CreateAsync(user, registerRequest.Password);
            if (!result.Succeeded)
            {
                var error = result.Errors.First();
                return Result.Failure(
                    new Error(error.Code, error.Description,
                    StatusCodes.Status400BadRequest)
                );
            }

            return Result.Success();
        }

        public async Task<Result> RegisterAsInstructorAsync(InstructorRegisterRequest registerRequest, CancellationToken cancellationToken = default)
        {
            //check if email exists 
            var isEmailExists = await _userManager.Users.AnyAsync(u => u.Email == registerRequest.Email, cancellationToken);
            if (isEmailExists)
                return Result.Failure(UserErrors.UserDublicatedEmail);

            var user = registerRequest.Adapt<AppUser>();
            user.UserType = UserType.Instructor;
            user.Instructor = new Instructor
            {
                Age = registerRequest.Age
            };

            var result = await _userManager.CreateAsync(user, registerRequest.Password);
            if (!result.Succeeded)
            {
                var error = result.Errors.First();
                return Result.Failure(
                    new Error(error.Code, error.Description,
                    StatusCodes.Status400BadRequest)
                );
            }

            return Result.Success();
        }

        public async Task<Result> RegisterAsStudentAsync(StudentRegisterRequest registerRequest, CancellationToken cancellationToken = default)
        {
            //check if email exists 
            var isEmailExists = await _userManager.Users.AnyAsync(u => u.Email == registerRequest.Email, cancellationToken);
            if (isEmailExists)
                return Result.Failure(UserErrors.UserDublicatedEmail);

            var user = registerRequest.Adapt<AppUser>();
            user.UserType = UserType.Student;
            user.Student = new Student
            {
                Address = registerRequest.Address
            };

            var result = await _userManager.CreateAsync(user, registerRequest.Password);
            if (!result.Succeeded)
            {
                var error = result.Errors.First();
                return Result.Failure(
                    new Error(error.Code, error.Description,
                    StatusCodes.Status400BadRequest)
                );
            }

            return Result.Success();
        }

        public async Task<Result> RevokeRefreshAsync(string token, string refreshToken, CancellationToken cancellationToken = default)
        {
            var userId = _jwtProvider.ValidateToken(token);
            if (userId is null)
                return Result.Failure(UserErrors.UserInvalidAccessToken);

            var user = await _userManager.FindByIdAsync(userId);
            if (user is null)
                return Result.Failure(UserErrors.UserNotFound);

            var userRefreshToken = user.RefreshTokens.SingleOrDefault(x => x.Token == refreshToken && x.IsActive);
            if (userRefreshToken is null)
                return Result.Failure(UserErrors.UserInvalidResreshToken);

            userRefreshToken.RevokedOn = DateTime.UtcNow;
            await _userManager.UpdateAsync(user);

            return Result.Success();
        }

        private static string GenerateRefreshToken()
        {
            var number = RandomNumberGenerator.GetBytes(64);
            var token = Convert.ToBase64String(number);

            return token;
        }
    }
}
