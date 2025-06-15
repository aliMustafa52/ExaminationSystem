using ExaminationSystem.Abstractions;

namespace ExaminationSystem.Errors
{
    public static class QuestionErrors
    {
        public static readonly Error QuestionNotFound =
            new("Question.NotFound", "No Question was found with the given ID", StatusCodes.Status404NotFound);
    }
}
