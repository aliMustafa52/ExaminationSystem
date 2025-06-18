using ExaminationSystem.Abstractions;

namespace ExaminationSystem.Errors
{
    public static class StudentErrors
    {
        public static readonly Error StudentNotFound =
            new("Student.NotFound", "No Student was found with the given ID", StatusCodes.Status404NotFound);

        public static readonly Error StudentSubmitedExamBefore =
            new("Student.SubmitedExamBefore", "this student submited this exam before", StatusCodes.Status404NotFound);

        public static readonly Error StudentNotYetSubmittedForReview =
            new("Student.NotYetSubmittedForReview", "This exam is Not Yet Submitted For Review", StatusCodes.Status404NotFound);
    }
}
