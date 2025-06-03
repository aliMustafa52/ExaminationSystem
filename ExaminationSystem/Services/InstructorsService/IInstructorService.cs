using ExaminationSystem.Abstractions;
using ExaminationSystem.Contracts.Instructors;

namespace ExaminationSystem.Services.InstructorsService
{
    public interface IInstructorService
    {
        Task<IEnumerable<InstructorResponse>> GetAllAsync();

        Task<Result<InstructorResponseWithCourses>> GetByIdAsync(int id);
        Task<InstructorResponse> AddInstructorAsync(InstructorRequest request);
        Task<Result> UpdateInstructorAsync(int id, InstructorRequest request);

        Task<Result> DeleteInstructorAsync(int id, CancellationToken cancellationToken);
    }
}
