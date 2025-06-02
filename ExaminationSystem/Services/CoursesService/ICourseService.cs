using ExaminationSystem.Abstractions;
using ExaminationSystem.Contracts.Courses;
using ExaminationSystem.Entities;

namespace ExaminationSystem.Services.CoursesService
{
    public interface ICourseService
    {
        Task<IEnumerable<CourseResponse>> GetAllAsync();

        Task<IEnumerable<CourseResponse>> GetAsync(int? courseId, string? courseName, int? courseHours);

        Task<Result<CourseResponse>> GetByIdAsync(int id);

        Task<Result<CourseResponse>> AddCourseAsync(CourseRequest request);

        Task<Result> UpdateCourseAsync(int id, CourseRequest request);

        Task<Result> DeleteCourseAsync(int id, CancellationToken cancellationToken);

    }
}
