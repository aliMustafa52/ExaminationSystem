using ExaminationSystem.Contracts.Courses;

namespace ExaminationSystem.Contracts.Instructors
{
    public record InstructorResponseWithCourses(
        int Id, 
        string Name, 
        int Age,
        IEnumerable<CourseResponse> Courses
    );
}
