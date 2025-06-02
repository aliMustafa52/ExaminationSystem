using ExaminationSystem.Abstractions;
using ExaminationSystem.Contracts.Instructors;

namespace ExaminationSystem.Services.InstructorsService
{
    public interface IInstructorService
    {
        Task<IEnumerable<InstructorResponse>> GetAllAsync();

        Task<Result<InstructorResponseWithCourses>> GetByIdAsync(int id);
        Task<InstructorResponse> AddCourseAsync(InstructorRequest request);
        Task<Result> UpdateCourseAsync(int id, InstructorRequest request);

        Task<Result> DeleteCourseAsync(int id, CancellationToken cancellationToken);
    }
}
