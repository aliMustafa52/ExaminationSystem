using FluentValidation;

namespace ExaminationSystem.Contracts.Exams
{
    public class ExamQuestionRequestValidator : AbstractValidator<ExamQuestionsRequest>
    {
        public ExamQuestionRequestValidator()
        {
            RuleForEach(x => x.QuestionIds)
                .GreaterThan(0)
                .WithMessage("Each number must be greater than 0.");
        }
    }
}
