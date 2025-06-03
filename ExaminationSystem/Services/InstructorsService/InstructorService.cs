using ExaminationSystem.Abstractions;
using ExaminationSystem.Contracts.Courses;
using ExaminationSystem.Contracts.Instructors;
using ExaminationSystem.Entities;
using ExaminationSystem.Errors;
using ExaminationSystem.Repositories;
using Mapster;
using Microsoft.EntityFrameworkCore;

namespace ExaminationSystem.Services.InstructorsService
{
    public class InstructorService(IGeneralRepository<Instructor> generalRepository) : IInstructorService
    {
        private readonly IGeneralRepository<Instructor> _generalRepository = generalRepository;

        public async Task<IEnumerable<InstructorResponse>> GetAllAsync()
        {
            var courses = await _generalRepository.GetAll().ToListAsync();

            return courses.Adapt<IEnumerable<InstructorResponse>>();
        }

        public async Task<Result<InstructorResponseWithCourses>> GetByIdAsync(int id)
        {
            var instructor = await _generalRepository.GetByIdAsync(id, i => i.Courses);
            if (instructor is null)
                return Result.Failure<InstructorResponseWithCourses>(InstructorErrors.InstructorNotFound);

            var instructorResponse = instructor.Adapt<InstructorResponseWithCourses>();

            return Result.Success(instructorResponse);
        }

        public async Task<InstructorResponse> AddInstructorAsync(InstructorRequest request)
        {
            var instructor = request.Adapt<Instructor>();
            var createdInstructor = await _generalRepository.AddAsync(instructor);

            var instructorResponse = createdInstructor.Adapt<InstructorResponse>();

            return instructorResponse;
        }

        public async Task<Result> UpdateInstructorAsync(int id, InstructorRequest request)
        {
            var updatedRows = await _generalRepository.UpdateAsync(c => c.Id == id,
                s => s
                    //.SetProperty(c => c.Name, request.Name)
                    .SetProperty(c => c.Age, request.Age));

            if (updatedRows == 0)
                return Result.Failure(InstructorErrors.InstructorNotFound);

            return Result.Success();
        }

        public async Task<Result> DeleteInstructorAsync(int id, CancellationToken cancellationToken)
        {
            var isDeleted = await _generalRepository.DeleteAsync(id);
            if (!isDeleted)
                return Result.Failure(InstructorErrors.InstructorNotFound);

            return Result.Success();
        }


    }
}
