using ExaminationSystem.Abstractions;

namespace ExaminationSystem.Errors
{
    public static class InstructorErrors
    {
        public static readonly Error InstructorNotFound =
            new("Instructor.NotFound", "No Instructor was found with the given ID", StatusCodes.Status404NotFound);

        public static readonly Error InstructorNotSaved =
            new("Instructor.NotSaved", "Cannot save Instructor at the time", StatusCodes.Status400BadRequest);
    }
}
