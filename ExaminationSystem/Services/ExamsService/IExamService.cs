using ExaminationSystem.Abstractions;
using ExaminationSystem.Contracts.Exams;

namespace ExaminationSystem.Services.ExamsService
{
    public interface IExamService
    {
        Task<Result<IEnumerable<ExamResponse>>> GetAllAsync(int courseId, CancellationToken cancellationToken = default);
        Task<Result<IEnumerable<ExamResponse>>> GetAllForTeacherAsync(int courseId, string instructorId, CancellationToken cancellationToken = default);

        Task<Result<ExamResponse>> GetByIdAsync(int courseId, int examId, CancellationToken cancellationToken = default);
        Task<Result<ExamResponse>> GetByIdForTeacherAsync(int courseId, int examId, string instructorId, CancellationToken cancellationToken = default);

        Task<Result<ExamResponse>> AddExamAsync(int courseId, string instructorId, AddExamRequest request, CancellationToken cancellationToken = default);

        Task<Result> UpdateExamAsync(int courseId, int examId, string instructorId, UpdateExamRequest request, CancellationToken cancellationToken = default);

        Task<Result> DeleteExamAsync(int courseId, int examId, string instructorId, CancellationToken cancellationToken = default);
    }
}
