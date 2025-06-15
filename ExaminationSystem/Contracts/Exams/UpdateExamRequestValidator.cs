using FluentValidation;

namespace ExaminationSystem.Contracts.Exams
{
    public class UpdateExamRequestValidator : AbstractValidator<UpdateExamRequest>
    {
        public UpdateExamRequestValidator()
        {
            RuleFor(x => x.Name)
                .NotEmpty()
                .Length(4, 30);

            RuleFor(x => x.Description)
                .NotEmpty()
                .Length(4, 300);
        }
    }
}
