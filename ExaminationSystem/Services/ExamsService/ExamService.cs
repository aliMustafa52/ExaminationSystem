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

        public async Task<Result<IEnumerable<ExamResponse>>> GetAllForStudentAsync(string studentId, CancellationToken cancellationToken = default)
        {
            var studentResult = await GetAndValidateStudentAsync(studentId, cancellationToken);
            if (studentResult.IsFailure)
                return Result.Failure<IEnumerable<ExamResponse>>(studentResult.Error);

            var exams = await _studentExamRepository.Get(se => se.StudentId == studentResult.Value.Id)
                            .Select(eu => eu.Exam)
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

        public async Task<Result<ExamResponseWithQuestions>> GetByIdForStudentAsync(int examId, string studentId, CancellationToken cancellationToken = default)
        {
            var studentResult = await GetAndValidateStudentAsync(studentId, cancellationToken);
            if (studentResult.IsFailure)
                return Result.Failure<ExamResponseWithQuestions>(studentResult.Error);

            var exam = await _studentExamRepository
                    .Get(se => se.StudentId == studentResult.Value.Id && se.ExamId == examId)
                    .Select(se => new ExamResponseWithQuestions(
                        se.Exam.Id,
                        se.Exam.Name,
                        se.Exam.Description,
                        se.Exam.ExamType,
                        se.Exam.Duration,
                        se.Exam.NumberOfQuestions,
                        se.Exam.CourseId,
                        se.Exam.InstructorId,
                        se.Exam.ExamQuestions.Select(x => new QuestionInExamResponse(
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
            var instructorResult = await GetAndValidateInstructorAsync(instructorId, cancellationToken);
            if (instructorResult.IsFailure)
                return Result.Failure<ExamResponse>(instructorResult.Error);

            var isCourseExists = await _courseRepository.AnyAsync(c => 
                    c.Id == courseId &&
                    c.IsActive && 
                    c.InstructorId == instructorResult.Value.Id, cancellationToken);
            if (!isCourseExists)
                return Result.Failure<ExamResponse>(CourseErrors.CourseNotFound);

            //check if final already exists
            if(request.ExamType == ExamType.Final)
            {
                var isFinalExists = await _examRepository.AnyAsync(e => 
                        e.ExamType == ExamType.Final &&
                        e.InstructorId == instructorResult.Value.Id &&
                        e.CourseId == courseId &&
                        e.IsActive, cancellationToken);
                if(isFinalExists)
                    return Result.Failure<ExamResponse>(ExamErrors.ExamOnlyOneFinal);
            }

            var exam = request.Adapt<Exam>();
            exam.CourseId = courseId;
            exam.InstructorId = instructorResult.Value.Id;

            var createdExam = await _examRepository.AddAsync(exam);

            var examResponse = createdExam.Adapt<ExamResponse>();

            return Result.Success(examResponse);
        }

        public async Task<Result> UpdateExamAsync(int courseId, int examId, string instructorId, UpdateExamRequest request, CancellationToken cancellationToken = default)
        {
            var instructorResult = await GetAndValidateInstructorAsync(instructorId, cancellationToken);
            if (instructorResult.IsFailure)
                return Result.Failure(instructorResult.Error);

            var isCourseExists = await _courseRepository.AnyAsync(c =>
                    c.Id == courseId &&
                    c.IsActive &&
                    c.InstructorId == instructorResult.Value.Id, cancellationToken);
            if (!isCourseExists)
                return Result.Failure<ExamResponse>(CourseErrors.CourseNotFound);

            var isExamExists = await _examRepository.AnyAsync(e => e.Id == examId &&
                    e.CourseId == courseId &&
                    e.InstructorId == instructorResult.Value.Id &&
                    e.IsActive, cancellationToken);
            if(!isExamExists)
                return Result.Failure<ExamResponse>(ExamErrors.ExamNotFound);

            await _examRepository.UpdateAsync(e => e.Id == examId && e.InstructorId == instructorResult.Value.Id && e.IsActive,
                s => s
                    .SetProperty(e => e.Name, request.Name)
                    .SetProperty(e => e.Description, request.Description)
                    .SetProperty(e => e.Duration, request.Duration)
                    );

            return Result.Success();
        }

        public async Task<Result> DeleteExamAsync(int courseId, int examId, string instructorId, CancellationToken cancellationToken = default)
        {
            var instructorResult = await GetAndValidateInstructorAsync(instructorId, cancellationToken);
            if (instructorResult.IsFailure)
                return Result.Failure(instructorResult.Error);

            var isCourseExists = await _courseRepository.AnyAsync(c =>
                    c.Id == courseId &&
                    c.IsActive &&
                    c.InstructorId == instructorResult.Value.Id, cancellationToken);
            if (!isCourseExists)
                return Result.Failure<ExamResponse>(CourseErrors.CourseNotFound);

            var isExamExists = await _examRepository.AnyAsync(e => e.Id == examId &&
                                            e.CourseId == courseId &&
                                            e.InstructorId == instructorResult.Value.Id &&
                                            e.IsActive, cancellationToken);
            if (!isExamExists)
                return Result.Failure<ExamResponse>(ExamErrors.ExamNotFound);

            var isDeleted = await _examRepository.DeleteAsync(examId);
            if (!isDeleted)
                return Result.Failure(CourseErrors.CourseNotFound);

            return Result.Success();
        }

        public async Task<Result> AssignQuestionToExam(int examId, string instructorId, ExamQuestionsRequest request, CancellationToken cancellationToken = default)
        {
            var instructorResult = await GetAndValidateInstructorAsync(instructorId, cancellationToken);
            if (instructorResult.IsFailure)
                return Result.Failure(instructorResult.Error);

            var isExamExists = await _examRepository.AnyAsync(e => e.Id == examId && e.InstructorId == instructorResult.Value.Id && e.IsActive, cancellationToken);
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
            var instructorResult = await GetAndValidateInstructorAsync(instructorId, cancellationToken);
            if (instructorResult.IsFailure)
                return Result.Failure(instructorResult.Error);

            var isExamExists = await _examRepository.AnyAsync(e => e.Id == examId && e.InstructorId == instructorResult.Value.Id && e.IsActive, cancellationToken);
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

        public async Task<Result> AssignRandomQuestionsToExam(int examId, string instructorId, AutoExamQuestionsRequest request, CancellationToken cancellationToken = default)
        {
            // Step 1: Authorize the instructor and validate they own the target exam.
            var instructorResult = await GetAndValidateInstructorAsync(instructorId, cancellationToken);
            if (instructorResult.IsFailure)
                return Result.Failure(instructorResult.Error);

            var exam = await _examRepository.Get(e => e.Id == examId && e.InstructorId == instructorResult.Value.Id && e.IsActive)
                            .SingleOrDefaultAsync(cancellationToken);
            if (exam is null)
                return Result.Failure(ExamErrors.ExamNotFound);

            // Step 2: Ensure a clean slate by deleting all previously assigned questions.
            //remove any old questions
            var existingQuestions = await _examQuestionRepository.Get(eq => eq.ExamId == examId)
                .ToListAsync(cancellationToken);
            if (existingQuestions.Count != 0)
                await _examQuestionRepository.Get(eq => eq.ExamId == examId)
                        .ExecuteDeleteAsync(cancellationToken);

            // Step 3: Validate that the request's question counts match the exam's configuration.
            //is sent number of questions the same as actual number of questions
            var totalSentQuestions = request.MediumQuestions + request.EasyQuestions + request.HardQuestions;
            if(exam.NumberOfQuestions != totalSentQuestions)
                return Result.Failure(ExamErrors.ExamQuestionNumberMismatch);

            // Step 4: Prepare tasks to fetch random question IDs for each difficulty level.
            var EasyQuestionIds = await _questionRepository.Get(q => q.IsActive && q.DifficultyLevel == QuestionDifficulty.Easy)
                        .OrderBy(_ => Guid.NewGuid())
                        .Take(request.EasyQuestions)
                        .Select(q => q.Id)
                        .ToListAsync(cancellationToken);

            var MediumQuestionIds = await _questionRepository.Get(q => q.IsActive && q.DifficultyLevel == QuestionDifficulty.Medium)
                            .OrderBy(_ => Guid.NewGuid())
                            .Take(request.MediumQuestions)
                            .Select(q => q.Id)
                            .ToListAsync(cancellationToken);

            var HardQuestionIds = await _questionRepository.Get(q => q.IsActive && q.DifficultyLevel == QuestionDifficulty.Hard)
                            .OrderBy(_ => Guid.NewGuid())
                            .Take(request.HardQuestions)
                            .Select(q => q.Id)
                            .ToListAsync(cancellationToken);

            // Step 6: Get the results and verify that the database had enough questions to fulfill the request.
            if (EasyQuestionIds.Count < request.EasyQuestions)
                return Result.Failure(ExamErrors.ExamInsufficientEasyQuestions);

            if (MediumQuestionIds.Count < request.MediumQuestions)
                return Result.Failure(ExamErrors.ExamInsufficientMediumQuestions);

            if (HardQuestionIds.Count < request.HardQuestions)
                return Result.Failure(ExamErrors.ExamInsufficientHardQuestions);

            // Step 7: Aggregate all the fetched question IDs into a single list.
            var allQuestionsId = new List<int>();
            allQuestionsId.AddRange(EasyQuestionIds);
            allQuestionsId.AddRange(MediumQuestionIds);
            allQuestionsId.AddRange(HardQuestionIds);

            var randomAllQuestionsId = allQuestionsId.OrderBy(_ => Guid.NewGuid());

            // Step 8: Create the new ExamQuestion link entities for the assignment.
            var examQuestions = randomAllQuestionsId.Select(questionId => new ExamQuestion
            {
                ExamId = examId,
                QuestionId = questionId
            }).ToList();

            await _examQuestionRepository.AddRangeAsync(examQuestions);

            return Result.Success();
        }

        public async Task<Result> AssignExamToStudent(int examId, int studentId, string instructorId, CancellationToken cancellationToken = default)
        {
            var instructorResult = await GetAndValidateInstructorAsync(instructorId, cancellationToken);
            if (instructorResult.IsFailure)
                return Result.Failure(instructorResult.Error);

            var examToAssign = await _examRepository.Get(e => 
                    e.Id == examId &&
                    e.InstructorId == instructorResult.Value.Id &&
                    e.IsActive)
                .Select(e => new {Exam = e, ActualQuestionCount = e.ExamQuestions.Count()})
                .FirstOrDefaultAsync(cancellationToken);
            if(examToAssign is null)
                return Result.Failure(ExamErrors.ExamNotFound);

            if (examToAssign.ActualQuestionCount == 0)
                return Result.Failure(ExamErrors.ExamIsEmpty);

            if (examToAssign.ActualQuestionCount != examToAssign.Exam.NumberOfQuestions)
                return Result.Failure(ExamErrors.ExamQuestionMismatch);

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
            var instructorResult = await GetAndValidateInstructorAsync(instructorId, cancellationToken);
            if (instructorResult.IsFailure)
                return Result.Failure(instructorResult.Error);

            var isExamExists = await _examRepository.AnyAsync(e => e.Id == examId && e.InstructorId == instructorResult.Value.Id && e.IsActive, cancellationToken);
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

        //public async Task<Result> SubmitExam(int examId, string studentId, SubmitExamRequest request, CancellationToken cancellationToken = default)
        //{
        //    var studentResult = await GetAndValidateStudentAsync(studentId, cancellationToken);
        //    if (studentResult.IsFailure)
        //        return Result.Failure<ExamResponseWithQuestions>(studentResult.Error);

        //    var exam = await _examRepository.Get(e => e.Id == examId && e.IsActive)
        //            .Select(e => new
        //            {
        //                questionIds = e.ExamQuestions.Select(eq => eq.Id),
        //                correctChoiceIds = e.ExamQuestions.SelectMany(eq => eq.Question.Choices.Select(c => c.IsCorrect))
        //            })
        //            .SingleOrDefaultAsync(cancellationToken);
        //    if (exam is null)
        //        return Result.Failure(ExamErrors.ExamNotFound);
        //}

        private async Task<Result<Instructor>> GetAndValidateInstructorAsync(string instructorId, CancellationToken cancellationToken)
        {
            var instructorUser = await _userManager.Users
                    .Where(u => u.Id == instructorId && u.UserType == UserType.Instructor && !u.IsDisabled)
                    .Select(u => u.Instructor)
                    .SingleOrDefaultAsync(cancellationToken);

            if (instructorUser is null || !instructorUser.IsActive)
                return Result.Failure<Instructor>(InstructorErrors.InstructorNotFound);

            return Result.Success(instructorUser);
        }

        private async Task<Result<Student>> GetAndValidateStudentAsync(string studentId, CancellationToken cancellationToken)
        {
            var studentUser = await _userManager.Users
                    .Where(u => u.Id == studentId && u.UserType == UserType.Student && !u.IsDisabled)
                    .Select(u => u.Student)
                    .SingleOrDefaultAsync(cancellationToken);

            if (studentUser is null || !studentUser.IsActive)
                return Result.Failure<Student>(StudentErrors.StudentNotFound);

            return Result.Success(studentUser);
        }


    }
}
