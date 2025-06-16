using ExaminationSystem.Abstractions;

namespace ExaminationSystem.Errors
{
    public static class StudentErrors
    {
        public static readonly Error StudentNotFound =
            new("Student.NotFound", "No Student was found with the given ID", StatusCodes.Status404NotFound);
    }
}
