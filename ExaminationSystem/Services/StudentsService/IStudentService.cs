using ExaminationSystem.Contracts.Students;

namespace ExaminationSystem.Services.StudentsService
{
    public interface IStudentService
    {
        Task<IEnumerable<StudentResponse>> GetAllAsync();
    }
}
