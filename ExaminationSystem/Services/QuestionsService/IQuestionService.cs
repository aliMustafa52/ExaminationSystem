using ExaminationSystem.Abstractions;
using ExaminationSystem.Contracts.Questions;

namespace ExaminationSystem.Services.QuestionsService
{
    public interface IQuestionService
    {
        Task<Result<IEnumerable<QuestionResponse>>> GetAllQuestionsAsync(string instructorId, CancellationToken cancellationToken = default);

        Task<Result<QuestionResponse>> GetQuestionByIdAsync(string instructorId, int questionId, CancellationToken cancellationToken = default);

        Task<Result<QuestionResponse>> AddQuestionAsync(string instructorId, AddQuestionRequest request, CancellationToken cancellationToken = default);

        Task<Result> UpdateQuestionAsync(string instructorId, int questionId, UpdateQuestionRequest request, CancellationToken cancellationToken = default);

        Task<Result> DeleteQuestionAsync(string instructorId, int questionId, CancellationToken cancellationToken = default);
    }
}
