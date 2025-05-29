namespace ExaminationSystem.Contracts.Courses
{
    public record CourseResponse
    (
        int Id,
        string Name,
        string Description,
        int Hours
    );
}
