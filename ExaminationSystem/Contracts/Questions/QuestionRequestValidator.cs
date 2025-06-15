using FluentValidation;

namespace ExaminationSystem.Contracts.Questions
{
    public class QuestionRequestValidator : AbstractValidator<AddQuestionRequest>
    {
        public QuestionRequestValidator()
        {
            RuleFor(x => x.Text)
                .NotEmpty()
                .Length(3, 30);

            RuleFor(x => x.Choices)
                .Must(x => x.Count() > 1)
                .WithMessage("Question should have At least two Choices")
                .When(x => x.Choices is not null);

            RuleFor(x => x.Choices)
                .Must(x => x.Distinct().Count() == x.Count())
                .WithMessage("You cannot add duplicated answers")
                .When(x => x.Choices is not null);

        }
    }
}
