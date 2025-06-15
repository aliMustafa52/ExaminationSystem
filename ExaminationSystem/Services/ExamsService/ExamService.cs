using ExaminationSystem.Abstractions;
using ExaminationSystem.Contracts.Courses;
using ExaminationSystem.Contracts.Exams;
using ExaminationSystem.Entities;
using ExaminationSystem.Entities.Enums;
using ExaminationSystem.Errors;
using ExaminationSystem.Repositories;
using Mapster;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;

namespace ExaminationSystem.Services.ExamsService
{
    public class ExamService(IGeneralRepository<Exam> examRepository,
        IGeneralRepository<Course> courseRepository,
        IGeneralRepository<Instructor> instructorRepository,
        UserManager<AppUser> userManager) : IExamService
    {
        private readonly IGeneralRepository<Exam> _examRepository = examRepository;
        private readonly IGeneralRepository<Course> _courseRepository = courseRepository;
        private readonly IGeneralRepository<Instructor> _instructorRepository = instructorRepository;
        private readonly UserManager<AppUser> _userManager = userManager;

        public async Task<Result<IEnumerable<ExamResponse>>> GetAllAsync(int courseId, CancellationToken cancellationToken = default)
        {
            var isCourseExists = await _courseRepository.AnyAsync(c =>
                    c.Id == courseId &&
                    c.IsActive, cancellationToken);
            if (!isCourseExists)
                return Result.Failure<IEnumerable<ExamResponse>>(CourseErrors.CourseNotFound);

            var exams = await _examRepository.Get(e => e.CourseId == courseId)
                    .ToListAsync(cancellationToken);

            var response = exams.Adapt<IEnumerable<ExamResponse>>();

            return Result.Success(response);
        }

        public async Task<Result<IEnumerable<ExamResponse>>> GetAllForTeacherAsync(int courseId, string instructorId, CancellationToken cancellationToken = default)
        {
            var instructorUser = await _userManager.Users
                    .Include(x => x.Instructor)
                    .Where(u => u.Id == instructorId && u.UserType == UserType.Instructor && !u.IsDisabled)
                    .SingleOrDefaultAsync(cancellationToken);
            if (instructorUser is null)
                return Result.Failure<IEnumerable<ExamResponse>>(UserErrors.UserNotFound);

            if (instructorUser.Instructor is null || !instructorUser.Instructor.IsActive)
                return Result.Failure<IEnumerable<ExamResponse>>(InstructorErrors.InstructorNotFound);

            var isCourseExists = await _courseRepository.AnyAsync(c =>
                    c.Id == courseId &&
                    c.IsActive, cancellationToken);
            if (!isCourseExists)
                return Result.Failure<IEnumerable<ExamResponse>>(CourseErrors.CourseNotFound);

            var exams = await _examRepository.Get(e => e.CourseId == courseId && e.InstructorId == instructorUser.Instructor.Id)
                    .ToListAsync(cancellationToken);

            var response = exams.Adapt<IEnumerable<ExamResponse>>();

            return Result.Success(response);
        }

        public async Task<Result<ExamResponse>> GetByIdAsync(int courseId, int examId, CancellationToken cancellationToken = default)
        {
            var isCourseExists = await _courseRepository.AnyAsync(c =>
                    c.Id == courseId &&
                    c.IsActive, cancellationToken);
            if (!isCourseExists)
                return Result.Failure<ExamResponse>(CourseErrors.CourseNotFound);

            var exam = await _examRepository.Get(e => e.Id == examId  && e.CourseId == courseId)
                    .FirstOrDefaultAsync(cancellationToken);
            if (exam is null)
                return Result.Failure<ExamResponse>(ExamErrors.ExamNotFound);

            var response = exam.Adapt<ExamResponse>();

            return Result.Success(response);
        }

        public async Task<Result<ExamResponse>> GetByIdForTeacherAsync(int courseId, int examId, string instructorId, CancellationToken cancellationToken = default)
        {
            var instructorUser = await _userManager.Users
                    .Include(x => x.Instructor)
                    .Where(u => u.Id == instructorId && u.UserType == UserType.Instructor && !u.IsDisabled)
                    .SingleOrDefaultAsync(cancellationToken);
            if (instructorUser is null)
                return Result.Failure<ExamResponse>(UserErrors.UserNotFound);

            if (instructorUser.Instructor is null || !instructorUser.Instructor.IsActive)
                return Result.Failure<ExamResponse>(InstructorErrors.InstructorNotFound);

            var isCourseExists = await _courseRepository.AnyAsync(c =>
                    c.Id == courseId &&
                    c.IsActive, cancellationToken);
            if (!isCourseExists)
                return Result.Failure<ExamResponse>(CourseErrors.CourseNotFound);

            var exam = await _examRepository.Get(e => e.Id == examId && e.CourseId == courseId && e.InstructorId == instructorUser.Instructor.Id)
                    .FirstOrDefaultAsync(cancellationToken);
            if (exam is null)
                return Result.Failure<ExamResponse>(ExamErrors.ExamNotFound);

            var response = exam.Adapt<ExamResponse>();

            return Result.Success(response);
        }

        public async Task<Result<ExamResponse>> AddExamAsync(int courseId, string instructorId, AddExamRequest request, CancellationToken cancellationToken = default)
        {
            var instructorUser = await _userManager.Users
                    .Include(x => x.Instructor)
                    .Where(u => u.Id == instructorId && u.UserType == UserType.Instructor && !u.IsDisabled)
                    .SingleOrDefaultAsync(cancellationToken);
            if (instructorUser is null)
                return Result.Failure<ExamResponse>(UserErrors.UserNotFound);

            if (instructorUser.Instructor is null || !instructorUser.Instructor.IsActive)
                return Result.Failure<ExamResponse>(InstructorErrors.InstructorNotFound);

            var isCourseExists = await _courseRepository.AnyAsync(c => 
                    c.Id == courseId &&
                    c.IsActive && 
                    c.InstructorId == instructorUser.Instructor.Id, cancellationToken);
            if (!isCourseExists)
                return Result.Failure<ExamResponse>(CourseErrors.CourseNotFound);

            var exam = request.Adapt<Exam>();
            exam.CourseId = courseId;
            exam.InstructorId = instructorUser.Instructor.Id;

            var createdExam = await _examRepository.AddAsync(exam);

            var examResponse = createdExam.Adapt<ExamResponse>();

            return Result.Success(examResponse);
        }

        public async Task<Result> UpdateExamAsync(int courseId, int examId, string instructorId, UpdateExamRequest request, CancellationToken cancellationToken = default)
        {
            var instructorUser = await _userManager.Users
                    .Include(x => x.Instructor)
                    .Where(u => u.Id == instructorId && u.UserType == UserType.Instructor && !u.IsDisabled)
                    .SingleOrDefaultAsync(cancellationToken);
            if (instructorUser is null)
                return Result.Failure<ExamResponse>(UserErrors.UserNotFound);

            if (instructorUser.Instructor is null || !instructorUser.Instructor.IsActive)
                return Result.Failure<ExamResponse>(InstructorErrors.InstructorNotFound);

            var isCourseExists = await _courseRepository.AnyAsync(c =>
                    c.Id == courseId &&
                    c.IsActive &&
                    c.InstructorId == instructorUser.Instructor.Id, cancellationToken);
            if (!isCourseExists)
                return Result.Failure<ExamResponse>(CourseErrors.CourseNotFound);

            var isExamExists = await _examRepository.AnyAsync(e => e.Id == examId && e.InstructorId == instructorUser.Instructor.Id && e.IsActive, cancellationToken);
            if(!isExamExists)
                return Result.Failure<ExamResponse>(ExamErrors.ExamNotFound);

            await _examRepository.UpdateAsync(e => e.Id == examId && e.InstructorId == instructorUser.Instructor.Id && e.IsActive,
                s => s
                    .SetProperty(e => e.Name, request.Name)
                    .SetProperty(e => e.Description, request.Description)
                    .SetProperty(e => e.Duration, request.Duration)
                    );

            return Result.Success();
        }

        public async Task<Result> DeleteExamAsync(int courseId, int examId, string instructorId, CancellationToken cancellationToken = default)
        {
            var instructorUser = await _userManager.Users
                    .Include(x => x.Instructor)
                    .Where(u => u.Id == instructorId && u.UserType == UserType.Instructor && !u.IsDisabled)
                    .SingleOrDefaultAsync(cancellationToken);
            if (instructorUser is null)
                return Result.Failure<ExamResponse>(UserErrors.UserNotFound);

            if (instructorUser.Instructor is null || !instructorUser.Instructor.IsActive)
                return Result.Failure<ExamResponse>(InstructorErrors.InstructorNotFound);

            var isCourseExists = await _courseRepository.AnyAsync(c =>
                    c.Id == courseId &&
                    c.IsActive &&
                    c.InstructorId == instructorUser.Instructor.Id, cancellationToken);
            if (!isCourseExists)
                return Result.Failure<ExamResponse>(CourseErrors.CourseNotFound);

            var isExamExists = await _examRepository.AnyAsync(e => e.Id == examId && e.InstructorId == instructorUser.Instructor.Id && e.IsActive, cancellationToken);
            if (!isExamExists)
                return Result.Failure<ExamResponse>(ExamErrors.ExamNotFound);

            var isDeleted = await _examRepository.DeleteAsync(examId);
            if (!isDeleted)
                return Result.Failure(CourseErrors.CourseNotFound);

            return Result.Success();
        }
    }
}
