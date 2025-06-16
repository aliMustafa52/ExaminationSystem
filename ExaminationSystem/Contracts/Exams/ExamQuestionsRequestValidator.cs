using FluentValidation;

namespace ExaminationSystem.Contracts.Exams
{
    public class ExamQuestionsRequestValidator : AbstractValidator<ExamQuestionsRequest>
    {
        public ExamQuestionsRequestValidator()
        {
            RuleForEach(x => x.QuestionIds)
                .GreaterThan(0)
                .WithMessage("Each number must be greater than 0.");
        }
    }
}
