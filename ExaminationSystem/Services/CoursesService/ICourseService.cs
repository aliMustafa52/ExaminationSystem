using ExaminationSystem.Abstractions;
using ExaminationSystem.Contracts.Courses;
using ExaminationSystem.Entities;

namespace ExaminationSystem.Services.CoursesService
{
    public interface ICourseService
    {
        Task<IEnumerable<CourseResponse>> GetAllAsync();

        Task<IEnumerable<CourseResponse>> GetAsync(int? courseId, string? courseName, int? courseHours);
        Task<Result<IEnumerable<CourseResponse>>> GetOwnCoursesAsync(string instructorUserId, CancellationToken cancellationToken = default);

        Task<Result<CourseResponse>> GetByIdAsync(int id);

        Task<Result<CourseResponse>> AddCourseAsync(string instructorUserId, CourseRequest request);

        Task<Result> UpdateCourseAsync(int id, string instructorUserId, CourseRequest request);

        Task<Result> DeleteCourseAsync(int id, string instructorUserId, CancellationToken cancellationToken);
        Task<Result> AssignCourseToStudent(string instructorUserId, int stidentId, int courseId, CancellationToken cancellationToken = default);

    }
}
