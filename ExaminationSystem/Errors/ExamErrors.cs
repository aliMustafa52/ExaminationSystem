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
    }
}
