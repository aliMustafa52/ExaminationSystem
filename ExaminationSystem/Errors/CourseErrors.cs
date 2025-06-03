using ExaminationSystem.Abstractions;

namespace ExaminationSystem.Errors
{
    public static class CourseErrors
    {
        public static readonly Error CourseNotFound =
            new("Course.NotFound", "No Course was found with the given ID", StatusCodes.Status404NotFound);

        public static readonly Error CourseNotSaved =
            new("Course.NotSaved", "Cannot save course at the time", StatusCodes.Status400BadRequest);

        public static readonly Error CourseNotModified =
            new("Course.CannotModify", "Cannot Modify this course", StatusCodes.Status400BadRequest);

        public static readonly Error StudentAlreadyEnrolled =
            new("Course.StudentAlreadyEnrolled", "Student Already Enrolled to this course", StatusCodes.Status400BadRequest);
    }
}
