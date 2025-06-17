using ExaminationSystem.Abstractions;

namespace ExaminationSystem.Errors
{
    public static class ExamErrors
    {
        public static readonly Error ExamNotFound =
            new("Exam.NotFound", "No Exam was found with the given ID", StatusCodes.Status404NotFound);

        public static readonly Error ExamNotSaved =
            new("Exam.NotSaved", "Cannot save Exam at the time", StatusCodes.Status400BadRequest);

        public static readonly Error ExamNotModified =
            new("Exam.CannotModify", "Cannot Modify this Exam", StatusCodes.Status400BadRequest);

        public static readonly Error StudentAlreadyEnrolled =
            new("Exam.StudentAlreadyEnrolled", "Student Already Enrolled to this Exam", StatusCodes.Status400BadRequest);

        public static readonly Error StudentNotEnrolled =
            new("Exam.StudentNotEnrolled", "Student Not Enrolled to this Exam", StatusCodes.Status400BadRequest);
        
        public static readonly Error ExamIsEmpty =
            new("Exam.IsEmpty", "Exam Is Empty", StatusCodes.Status400BadRequest);

        public static readonly Error ExamOnlyOneFinal =
            new("Exam.OnlyOneFinal", "Final exam for this course is already exists", StatusCodes.Status400BadRequest);

        public static readonly Error ExamQuestionMismatch =
            new("Exam.QuestionMismatch", "Exam isn't ready yet (Assign more questions)", StatusCodes.Status400BadRequest);

        public static readonly Error ExamQuestionNumberMismatch =
            new("Exam.QuestionNumberMismatch", "Sent Number Of Questions are diffent from Exam Number of questions", StatusCodes.Status400BadRequest);

        public static readonly Error ExamInsufficientEasyQuestions =
            new("Exam.InsufficientEasyQuestions", "Not Enough Easy Questions", StatusCodes.Status400BadRequest);

        public static readonly Error ExamInsufficientMediumQuestions =
            new("Exam.InsufficientMediumQuestions", "Not Enough Medium Questions", StatusCodes.Status400BadRequest);

        public static readonly Error ExamInsufficientHardQuestions =
            new("Exam.InsufficientHardQuestions", "Not Enough Hard Questions", StatusCodes.Status400BadRequest);
    }
}
