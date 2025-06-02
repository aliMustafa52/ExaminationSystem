using ExaminationSystem.Abstractions;
using ExaminationSystem.Contracts.Courses;
using ExaminationSystem.Entities;
using ExaminationSystem.Errors;
using ExaminationSystem.Repositories;
using Mapster;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PredicateExtensions;
using System.Linq.Expressions;

namespace ExaminationSystem.Services.CoursesService
{
    public class CourseService(IGeneralRepository<Course> generalRepository,
        IGeneralRepository<Instructor> instructorRepository) : ICourseService
    {
        private readonly IGeneralRepository<Course> _generalRepository = generalRepository;
        private readonly IGeneralRepository<Instructor> _instructorRepository = instructorRepository;

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

        public async Task<Result<CourseResponse>> AddCourseAsync(CourseRequest request)
        {
            var instructor = await _instructorRepository.GetByIdAsync(request.InstructorId);
            if(instructor is null)
                return Result.Failure<CourseResponse>(InstructorErrors.InstructorNotFound);

            var course = request.Adapt<Course>();
            var createdCourse = await _generalRepository.AddAsync(course);

            var courseResponse = createdCourse.Adapt<CourseResponse>();

            return Result.Success(courseResponse);
        }

        public async Task<Result> UpdateCourseAsync(int id, CourseRequest request)
        {
            var updatedRows = await _generalRepository.UpdateAsync(c => c.Id == id,
                s => s
                    .SetProperty(c => c.Name, request.Name)
                    .SetProperty(c => c.Description, request.Description)
                    .SetProperty(c => c.Hours, request.Hours));

            if (updatedRows == 0)
                return Result.Failure(CourseErrors.CourseNotFound);

            return Result.Success();
        }

        public async Task<Result> DeleteCourseAsync(int id, CancellationToken cancellationToken)
        {
            var isDeleted = await _generalRepository.DeleteAsync(id);
            if (!isDeleted)
                return Result.Failure(CourseErrors.CourseNotFound);

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
