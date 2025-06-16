using ExaminationSystem.Abstractions;
using ExaminationSystem.Contracts.Choices;
using ExaminationSystem.Contracts.Courses;
using ExaminationSystem.Contracts.Exams;
using ExaminationSystem.Contracts.Questions;
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
        IGeneralRepository<ExamQuestion> ExamQuestionRepository,
        IGeneralRepository<Question> QuestionRepository,
        IGeneralRepository<Student> StudentRepository,
        IGeneralRepository<StudentExam> StudentExamRepository,
        UserManager<AppUser> userManager) : IExamService
    {
        private readonly IGeneralRepository<Exam> _examRepository = examRepository;
        private readonly IGeneralRepository<Course> _courseRepository = courseRepository;
        private readonly IGeneralRepository<ExamQuestion> _examQuestionRepository = ExamQuestionRepository;
        private readonly IGeneralRepository<Question> _questionRepository = QuestionRepository;
        private readonly IGeneralRepository<Student> _studentRepository = StudentRepository;
        private readonly IGeneralRepository<StudentExam> _studentExamRepository = StudentExamRepository;
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
                .Where(u => u.Id == instructorId && u.UserType == UserType.Instructor && !u.IsDisabled)
                .Select(u => u.Instructor)
                .SingleOrDefaultAsync(cancellationToken);

            if (instructorUser is null || !instructorUser.IsActive)
                return Result.Failure<IEnumerable<ExamResponse>>(InstructorErrors.InstructorNotFound);

            var isCourseExists = await _courseRepository.AnyAsync(c =>
                    c.Id == courseId &&
                    c.IsActive, cancellationToken);
            if (!isCourseExists)
                return Result.Failure<IEnumerable<ExamResponse>>(CourseErrors.CourseNotFound);

            var exams = await _examRepository.Get(e => e.CourseId == courseId && e.InstructorId == instructorUser.Id)
                    .ToListAsync(cancellationToken);

            var response = exams.Adapt<IEnumerable<ExamResponse>>();

            return Result.Success(response);
        }

        public async Task<Result<ExamResponseWithQuestions>> GetByIdAsync(int courseId, int examId, CancellationToken cancellationToken = default)
        {
            //var isCourseExists = await _courseRepository.AnyAsync(c =>
            //        c.Id == courseId &&
            //        c.IsActive, cancellationToken);
            //if (!isCourseExists)
            //    return Result.Failure<ExamResponseWithQuestions>(CourseErrors.CourseNotFound);

            var exam = await _examRepository.Get(e => 
                        e.Id == examId  && 
                        e.CourseId == courseId &&
                        e.Course.IsActive)
                    .Select(e => new ExamResponseWithQuestions(
                        e.Id,
                        e.Name,
                        e.Description,
                        e.ExamType,
                        e.Duration,
                        e.NumberOfQuestions,
                        e.CourseId,
                        e.InstructorId,
                        e.ExamQuestions.Select(x => new QuestionInExamResponse(
                            x.Question.Id,
                            x.Question.Text,
                            x.Question.Choices.Select(z => new ChoiceInExamResponse(
                                z.Id,
                                z.Content
                            ))
                        ))
                    ))
                    .FirstOrDefaultAsync(cancellationToken);
            if (exam is null)
                return Result.Failure<ExamResponseWithQuestions>(ExamErrors.ExamNotFound);

            return Result.Success(exam);
        }

        public async Task<Result<ExamResponseWithQuestions>> GetByIdForTeacherAsync(int courseId, int examId, string instructorId, CancellationToken cancellationToken = default)
        {
            var instructorUser = await _userManager.Users
                .Where(u => u.Id == instructorId && u.UserType == UserType.Instructor && !u.IsDisabled)
                .Select(u => u.Instructor)
                .SingleOrDefaultAsync(cancellationToken);

            if (instructorUser is null || !instructorUser.IsActive)
                return Result.Failure<ExamResponseWithQuestions>(InstructorErrors.InstructorNotFound);

            var exam = await _examRepository
                    .Get(e => e.Id == examId &&
                        e.CourseId == courseId &&
                        e.Course.IsActive &&
                        e.InstructorId == instructorUser.Id)
                    .Select(e => new ExamResponseWithQuestions(
                        e.Id,
                        e.Name,
                        e.Description,
                        e.ExamType,
                        e.Duration,
                        e.NumberOfQuestions,
                        e.CourseId,
                        e.InstructorId,
                        e.ExamQuestions.Select(x => new QuestionInExamResponse(
                            x.Question.Id,
                            x.Question.Text,
                            x.Question.Choices.Select(z => new ChoiceInExamResponse(
                                z.Id,
                                z.Content
                            ))
                        ))
                    ))
                    .FirstOrDefaultAsync(cancellationToken);
            if (exam is null)
                return Result.Failure<ExamResponseWithQuestions>(ExamErrors.ExamNotFound);

            return Result.Success(exam);
        }

        public async Task<Result<ExamResponse>> AddExamAsync(int courseId, string instructorId, AddExamRequest request, CancellationToken cancellationToken = default)
        {
            var instructorUser = await _userManager.Users
                    .Where(u => u.Id == instructorId && u.UserType == UserType.Instructor && !u.IsDisabled)
                    .Select(u => u.Instructor)
                    .SingleOrDefaultAsync(cancellationToken);

            if (instructorUser is null || !instructorUser.IsActive)
                return Result.Failure<ExamResponse>(InstructorErrors.InstructorNotFound);

            var isCourseExists = await _courseRepository.AnyAsync(c => 
                    c.Id == courseId &&
                    c.IsActive && 
                    c.InstructorId == instructorUser.Id, cancellationToken);
            if (!isCourseExists)
                return Result.Failure<ExamResponse>(CourseErrors.CourseNotFound);

            var exam = request.Adapt<Exam>();
            exam.CourseId = courseId;
            exam.InstructorId = instructorUser.Id;

            var createdExam = await _examRepository.AddAsync(exam);

            var examResponse = createdExam.Adapt<ExamResponse>();

            return Result.Success(examResponse);
        }

        public async Task<Result> UpdateExamAsync(int courseId, int examId, string instructorId, UpdateExamRequest request, CancellationToken cancellationToken = default)
        {
            var instructorUser = await _userManager.Users
                    .Where(u => u.Id == instructorId && u.UserType == UserType.Instructor && !u.IsDisabled)
                    .Select(u => u.Instructor)
                    .SingleOrDefaultAsync(cancellationToken);

            if (instructorUser is null || !instructorUser.IsActive)
                return Result.Failure(InstructorErrors.InstructorNotFound);

            var isCourseExists = await _courseRepository.AnyAsync(c =>
                    c.Id == courseId &&
                    c.IsActive &&
                    c.InstructorId == instructorUser.Id, cancellationToken);
            if (!isCourseExists)
                return Result.Failure<ExamResponse>(CourseErrors.CourseNotFound);

            var isExamExists = await _examRepository.AnyAsync(e => e.Id == examId && e.InstructorId == instructorUser.Id && e.IsActive, cancellationToken);
            if(!isExamExists)
                return Result.Failure<ExamResponse>(ExamErrors.ExamNotFound);

            await _examRepository.UpdateAsync(e => e.Id == examId && e.InstructorId == instructorUser.Id && e.IsActive,
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
                    .Where(u => u.Id == instructorId && u.UserType == UserType.Instructor && !u.IsDisabled)
                    .Select(u => u.Instructor)
                    .SingleOrDefaultAsync(cancellationToken);

            if (instructorUser is null || !instructorUser.IsActive)
                return Result.Failure(InstructorErrors.InstructorNotFound);

            var isCourseExists = await _courseRepository.AnyAsync(c =>
                    c.Id == courseId &&
                    c.IsActive &&
                    c.InstructorId == instructorUser.Id, cancellationToken);
            if (!isCourseExists)
                return Result.Failure<ExamResponse>(CourseErrors.CourseNotFound);

            var isExamExists = await _examRepository.AnyAsync(e => e.Id == examId && e.InstructorId == instructorUser.Id && e.IsActive, cancellationToken);
            if (!isExamExists)
                return Result.Failure<ExamResponse>(ExamErrors.ExamNotFound);

            var isDeleted = await _examRepository.DeleteAsync(examId);
            if (!isDeleted)
                return Result.Failure(CourseErrors.CourseNotFound);

            return Result.Success();
        }

        public async Task<Result> AssignQuestionToExam(int examId, string instructorId, ExamQuestionsRequest request, CancellationToken cancellationToken = default)
        {
            var instructorUser = await _userManager.Users
                    .Where(u => u.Id == instructorId && u.UserType == UserType.Instructor && !u.IsDisabled)
                    .Select(u => u.Instructor)
                    .SingleOrDefaultAsync(cancellationToken);

            if (instructorUser is null || !instructorUser.IsActive)
                return Result.Failure(InstructorErrors.InstructorNotFound);

            var isExamExists = await _examRepository.AnyAsync(e => e.Id == examId && e.InstructorId == instructorUser.Id && e.IsActive, cancellationToken);
            if (!isExamExists)
                return Result.Failure(ExamErrors.ExamNotFound);

            //Validate Question Existence
            var validQuestionIds = await _questionRepository.Get(q => request.QuestionIds.Contains(q.Id))
                        .Select(q => q.Id)
                        .ToListAsync(cancellationToken);

            var invalidQuestionIds = request.QuestionIds.Except(validQuestionIds);
            if(invalidQuestionIds.Any())
                return Result.Failure(QuestionErrors.QuestionNotFound);

            //Prevent Duplicate Entries
            var existingQuestionIds = await _examQuestionRepository.Get(e => e.ExamId == examId && request.QuestionIds.Contains(e.QuestionId))
                                .Select(e => e.QuestionId)
                                .ToListAsync(cancellationToken);

            var newQuestionIds = request.QuestionIds.Except(existingQuestionIds).ToList();

            var examQuestions = new List<ExamQuestion>();

            foreach(var questionId in newQuestionIds)
            {
                examQuestions.Add(new ExamQuestion { ExamId = examId, QuestionId = questionId });
                //await _examQuestionRepository.AddAsync(new ExamQuestion { ExamId = examId, QuestionId = questionId });
            }

            await _examQuestionRepository.AddRangeAsync(examQuestions);

            return Result.Success();
        }

        public async Task<Result> RemoveQuestionFromExam(int examId, string instructorId, ExamQuestionsRequest request, CancellationToken cancellationToken = default)
        {
            var instructorUser = await _userManager.Users
                    .Where(u => u.Id == instructorId && u.UserType == UserType.Instructor && !u.IsDisabled)
                    .Select(u => u.Instructor)
                    .SingleOrDefaultAsync(cancellationToken);

            if (instructorUser is null || !instructorUser.IsActive)
                return Result.Failure(InstructorErrors.InstructorNotFound);

            var isExamExists = await _examRepository.AnyAsync(e => e.Id == examId && e.InstructorId == instructorUser.Id && e.IsActive, cancellationToken);
            if (!isExamExists)
                return Result.Failure(ExamErrors.ExamNotFound);

            //Prevent Duplicate Entries
            var examQuestionsToRemove = await _examQuestionRepository.Get(e => e.ExamId == examId && request.QuestionIds.Contains(e.QuestionId))
                                .ToListAsync(cancellationToken);

            if(examQuestionsToRemove.Count == 0)
                return Result.Success();


            await _examQuestionRepository.RemoveRangeAsync(examQuestionsToRemove);

            return Result.Success();
        }

        public async Task<Result> AssignExamToStudent(int examId, int studentId, string instructorId, CancellationToken cancellationToken = default)
        {
            var instructorUser = await _userManager.Users
                    .Where(u => u.Id == instructorId && u.UserType == UserType.Instructor && !u.IsDisabled)
                    .Select(u => u.Instructor)
                    .SingleOrDefaultAsync(cancellationToken);

            if (instructorUser is null || !instructorUser.IsActive)
                return Result.Failure(InstructorErrors.InstructorNotFound);

            var isExamExists = await _examRepository.AnyAsync(e => e.Id == examId && e.InstructorId == instructorUser.Id && e.IsActive, cancellationToken);
            if (!isExamExists)
                return Result.Failure(ExamErrors.ExamNotFound);

            var isStudentExists = await _studentRepository.AnyAsync(x => x.Id == studentId && x.IsActive, cancellationToken);
            if(!isStudentExists)
                return Result.Failure(StudentErrors.StudentNotFound);

            var studentExam = await _studentExamRepository.Get(x => x.ExamId == examId && x.StudentId == studentId)
                .SingleOrDefaultAsync(cancellationToken);

            if (studentExam is not null)
                return Result.Failure(ExamErrors.StudentAlreadyEnrolled);

            await _studentExamRepository.AddAsync(new StudentExam
            {
                ExamId = examId,
                StudentId = studentId
            });

            return Result.Success();
        }

        public async Task<Result> RemoveExamFromStudent(int examId, int studentId, string instructorId, CancellationToken cancellationToken = default)
        {
            var instructorUser = await _userManager.Users
                    .Where(u => u.Id == instructorId && u.UserType == UserType.Instructor && !u.IsDisabled)
                    .Select(u => u.Instructor)
                    .SingleOrDefaultAsync(cancellationToken);

            if (instructorUser is null || !instructorUser.IsActive)
                return Result.Failure(InstructorErrors.InstructorNotFound);

            var isExamExists = await _examRepository.AnyAsync(e => e.Id == examId && e.InstructorId == instructorUser.Id && e.IsActive, cancellationToken);
            if (!isExamExists)
                return Result.Failure(ExamErrors.ExamNotFound);

            var isStudentExists = await _studentRepository.AnyAsync(x => x.Id == studentId && x.IsActive, cancellationToken);
            if (!isStudentExists)
                return Result.Failure(StudentErrors.StudentNotFound);

            var studentExam= await _studentExamRepository.Get(x => x.ExamId == examId && x.StudentId == studentId)
                .SingleOrDefaultAsync(cancellationToken);

            if(studentExam is null)
                return Result.Failure(ExamErrors.StudentNotEnrolled);

            await _studentExamRepository.RemoveAsync(studentExam);

            return Result.Success();
        }
    }
}
