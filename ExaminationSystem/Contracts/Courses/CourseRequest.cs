namespace ExaminationSystem.Contracts.Courses
{
    public record CourseRequest
    (
        string Name,
        string Description,
        int Hours,
        int InstructorId
    );
}
