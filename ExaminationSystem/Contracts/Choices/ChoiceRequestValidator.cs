using ExaminationSystem.Contracts.Questions;
using FluentValidation;

namespace ExaminationSystem.Contracts.Choices
{
    public class ChoiceRequestValidator : AbstractValidator<AddChoiceRequest>
    {
        public ChoiceRequestValidator()
        {
            RuleFor(x => x.Content)
                .NotEmpty()
                .Length(2, 50);
        }
    }
}
