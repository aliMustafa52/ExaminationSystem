using ExaminationSystem.Abstractions;
using ExaminationSystem.Contracts.Authentication;
using ExaminationSystem.Contracts.Courses;
using ExaminationSystem.Entities;
using ExaminationSystem.Entities.Enums;
using ExaminationSystem.Errors;
using ExaminationSystem.Repositories;
using Mapster;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PredicateExtensions;
using System.Linq.Expressions;

namespace ExaminationSystem.Services.CoursesService
{
    public class CourseService(IGeneralRepository<Course> generalRepository,
        IGeneralRepository<Instructor> instructorRepository,
        UserManager<AppUser> userManager, IGeneralRepository<Student> studentRepository,
        IGeneralRepository<StudentCourse> StudentCourseRepository) : ICourseService
    {
        private readonly IGeneralRepository<Course> _generalRepository = generalRepository;
        private readonly IGeneralRepository<Instructor> _instructorRepository = instructorRepository;
        private readonly IGeneralRepository<Student> _studentRepository = studentRepository;
        private readonly IGeneralRepository<StudentCourse> _studentCourseRepository = StudentCourseRepository;
        private readonly UserManager<AppUser> _userManager = userManager;

        public async Task<IEnumerable<CourseResponse>> GetAllAsync()
        {
            var courses = await _generalRepository.GetAll().ToListAsync();

            return courses.Adapt<IEnumerable<CourseResponse>>();
        }

        public async Task<IEnumerable<CourseResponse>> GetAsync(int? courseId, string? courseName, int? courseHours)
        {
            //PredicateBuilder

            var predicate = PredicateBuilder(courseId, courseName, courseHours);
            var query = await _generalRepository.Get(predicate).ToListAsync();

            return query.Adapt<IEnumerable<CourseResponse>>();
        }

        public async Task<Result<CourseResponse>> GetByIdAsync(int id)
        {
            var course = await _generalRepository.GetByIdAsync(id);
            if (course is null)
                return Result.Failure<CourseResponse>(CourseErrors.CourseNotFound);

            var courseResponse = course.Adapt<CourseResponse>();

            return Result.Success(courseResponse);
        }

        public async Task<Result<CourseResponse>> AddCourseAsync(string instructorUserId, CourseRequest request)
        {
            var instructorUser = await _userManager.Users
                    .Where(u => u.Id == instructorUserId)
                    .Include(x => x.Instructor)
                    .SingleOrDefaultAsync();
            if (instructorUser is null)
                return Result.Failure<CourseResponse>(UserErrors.UserNotFound);

            if(instructorUser.UserType != UserType.Instructor)
                return Result.Failure<CourseResponse>(UserErrors.UserIsNotAnInstructor);

            if(instructorUser.Instructor is null)
                return Result.Failure<CourseResponse>(InstructorErrors.InstructorNotFound);

            var course = request.Adapt<Course>();
            course.InstructorId = instructorUser.Instructor.Id;
            var createdCourse = await _generalRepository.AddAsync(course);

            var courseResponse = createdCourse.Adapt<CourseResponse>();

            return Result.Success(courseResponse);
        }

        public async Task<Result> UpdateCourseAsync(int id, string instructorUserId, CourseRequest request)
        {
            var instructorUser = await _userManager.Users
                    .Where(u => u.Id == instructorUserId)
                    .Include(x => x.Instructor)
                    .SingleOrDefaultAsync();
            if (instructorUser is null)
                return Result.Failure(UserErrors.UserNotFound);

            if (instructorUser.UserType != UserType.Instructor)
                return Result.Failure(UserErrors.UserIsNotAnInstructor);

            if (instructorUser.Instructor is null)
                return Result.Failure(InstructorErrors.InstructorNotFound);

            var courseExistsAndIsOwned = await _generalRepository
                .AnyAsync(c => c.Id == id && c.InstructorId == instructorUser.Instructor.Id);
            if (!courseExistsAndIsOwned)
                return Result.Failure(CourseErrors.CourseNotFound);

            var updatedRows = await _generalRepository.UpdateAsync(c => c.Id == id,
                s => s
                    .SetProperty(c => c.Name, request.Name)
                    .SetProperty(c => c.Description, request.Description)
                    .SetProperty(c => c.Hours, request.Hours));

            if (updatedRows == 0)
                return Result.Failure(CourseErrors.CourseNotFound);

            return Result.Success();
        }

        public async Task<Result> DeleteCourseAsync(int id, string instructorUserId, CancellationToken cancellationToken)
        {
            var instructorUser = await _userManager.Users
                    .Where(u => u.Id == instructorUserId)
                    .Include(x => x.Instructor)
                    .SingleOrDefaultAsync();
            if (instructorUser is null)
                return Result.Failure(UserErrors.UserNotFound);

            if (instructorUser.UserType != UserType.Instructor)
                return Result.Failure(UserErrors.UserIsNotAnInstructor);

            if (instructorUser.Instructor is null)
                return Result.Failure(InstructorErrors.InstructorNotFound);

            var courseExistsAndIsOwned = await _generalRepository
                .AnyAsync(c => c.Id == id && c.InstructorId == instructorUser.Instructor.Id);
            if (!courseExistsAndIsOwned)
                return Result.Failure(CourseErrors.CourseNotFound);

            var isDeleted = await _generalRepository.DeleteAsync(id);
            if (!isDeleted)
                return Result.Failure(CourseErrors.CourseNotFound);

            return Result.Success();
        }

        public async Task<Result<IEnumerable<CourseResponse>>> GetOwnCoursesAsync(string instructorUserId, CancellationToken cancellationToken = default)
        {
            var instructorUser = await _userManager.Users
                    .Where(u => u.Id == instructorUserId)
                    .Include(x => x.Instructor)
                    .SingleOrDefaultAsync(cancellationToken);
            if (instructorUser is null)
                return Result.Failure<IEnumerable<CourseResponse>>(UserErrors.UserNotFound);

            if (instructorUser.UserType != UserType.Instructor)
                return Result.Failure<IEnumerable<CourseResponse>>(UserErrors.UserIsNotAnInstructor);

            if (instructorUser.Instructor is null)
                return Result.Failure<IEnumerable<CourseResponse>>(InstructorErrors.InstructorNotFound);

            var courses = await _generalRepository
                .Get(c => c.InstructorId == instructorUser.Instructor.Id)
                .ToListAsync(cancellationToken);

            var coursesResponse = courses.Adapt<IEnumerable<CourseResponse>>();
            return Result.Success(coursesResponse);
        }

        public async Task<Result> AssignCourseToStudent(string instructorUserId, int stidentId, int courseId, CancellationToken cancellationToken = default)
        {
            var instructorUser = await _userManager.Users
                    .Where(u => u.Id == instructorUserId)
                    .Include(x => x.Instructor)
                    .SingleOrDefaultAsync(cancellationToken);
            if (instructorUser is null)
                return Result.Failure(UserErrors.UserNotFound);

            if (instructorUser.UserType != UserType.Instructor)
                return Result.Failure(UserErrors.UserIsNotAnInstructor);

            if (instructorUser.Instructor is null)
                return Result.Failure(InstructorErrors.InstructorNotFound);

            var course = await _generalRepository
                .Get(c => c.Id == courseId && c.InstructorId == instructorUser.Instructor.Id)
                .SingleOrDefaultAsync(cancellationToken);
            if (course is null)
                return Result.Failure(CourseErrors.CourseNotFound);

            var student = await _studentRepository.GetByIdWithTrackingAsync(stidentId);
            if (student is null)
                return Result.Failure(UserErrors.UserNotFound);

            var isEnrollmentExists = await _studentCourseRepository
                .AnyAsync(s => s.StudentId == stidentId && s.CourseId == courseId, cancellationToken);
            if (isEnrollmentExists)
                return Result.Failure(CourseErrors.StudentAlreadyEnrolled);

            // add course to this student
            var studentCourse = new StudentCourse
            {
                StudentId = stidentId,
                CourseId = courseId,
            };
            await _studentCourseRepository.AddAsync(studentCourse);
            
            return Result.Success();
        }

        private Expression<Func<Course, bool>> PredicateBuilder(int? courseId, string? courseName, int? courseHours)
        {
            var predicate = PredicateExtensions.PredicateExtensions.Begin<Course>(true);
            if (courseId.HasValue)
            {
                predicate = predicate.And(x => x.Id == courseId);
            }
            if (courseHours.HasValue)
            {
                predicate = predicate.And(x => x.Hours >= courseHours);
            }
            if (!string.IsNullOrEmpty(courseName))
            {
                predicate = predicate.And(x => x.Name == courseName);
            }

            return predicate;
        }
    }
}
