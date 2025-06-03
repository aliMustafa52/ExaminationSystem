using ExaminationSystem.Abstractions.Consts;
using FluentValidation;

namespace ExaminationSystem.Contracts.Authentication
{
    public class RegisterRequestValidator : AbstractValidator<RegisterRequest>
    {
        public RegisterRequestValidator()
        {
            RuleFor(x => x.Email)
                .NotEmpty()
                .EmailAddress();

            RuleFor(x => x.Password)
                .NotEmpty()
                .Matches(RegexPatterns.Password)
                .WithMessage("Password must be at least 8 characters long and include: At least one uppercase letter (A-Z) At least one lowercase letter (a-z) At least one number (0-9) At least one special character (@$!%*?&)");

            RuleFor(x => x.FirstName)
                .NotEmpty()
                .Length(3, 100);

            RuleFor(x => x.LastName)
                .NotEmpty()
                .Length(3, 100);
        }
    }
}
