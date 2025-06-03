using Azure;
using ExaminationSystem.Abstractions;
using ExaminationSystem.Contracts.Courses;
using ExaminationSystem.Contracts.Instructors;
using ExaminationSystem.Services.CoursesService;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace ExaminationSystem.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CoursesController(ICourseService courseService) : ControllerBase
    {
        private readonly ICourseService _courseService = courseService;

        [HttpGet("")]
        public async Task<ActionResult<IEnumerable<CourseResponse>>> GetAll()
        {
            var response = await _courseService.GetAllAsync();

            return Ok(response);
        }

        [HttpGet("my-courses")]
        [Authorize]
        public async Task<ActionResult<IEnumerable<CourseResponse>>> GetOwned(CancellationToken cancellationToken)
        {
            var instructorId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
            var result = await _courseService.GetOwnCoursesAsync(instructorId, cancellationToken);

            return result.IsSuccess
                ? Ok(result.Value)
                : result.ToProblem();
        }

        [HttpGet("filter")]
        public async Task<ActionResult<IEnumerable<CourseResponse>>> Get(int? courseId, string? courseName, int? courseHours)
        {
            var response = await _courseService.GetAsync(courseId, courseName, courseHours);

            return Ok(response);
        }

        
        [HttpGet("{id}")]
        public async Task<ActionResult<CourseResponse>> GetById([FromRoute] int id)
        {
            var result = await _courseService.GetByIdAsync(id);

            return result.IsSuccess
                ? Ok(result.Value)
                : result.ToProblem();
        }

        [HttpPost("")]
        [Authorize]
        public async Task<ActionResult<CourseResponse>> AddCourse([FromBody] CourseRequest request)
        {
            var instructorId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
            var result = await _courseService.AddCourseAsync(instructorId, request);

            return result.IsSuccess
            ? CreatedAtAction(nameof(GetById), new { id = result.Value.Id }, result.Value)
            : result.ToProblem();
        }

        [HttpPut("assign-course-to-student")]
        [Authorize]
        public async Task<ActionResult<CourseResponse>> AssignCourseToStudent([FromBody] StudentCourseRequest request, CancellationToken cancellationToken = default)
        {
            var instructorId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
            var result = await _courseService.AssignCourseToStudent(instructorId, request.StudentId, request.CourseId, cancellationToken);

            return result.IsSuccess
            ? NoContent()
            : result.ToProblem();
        }

        [HttpPut("{id}")]
        [Authorize]
        public async Task<IActionResult> UpdateCourse([FromRoute] int id,[FromBody] CourseRequest request)
        {
            var instructorId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
            var result = await _courseService.UpdateCourseAsync(id, instructorId, request);

            return result.IsSuccess
                ? NoContent()
                : result.ToProblem();
        }


        [HttpDelete("{id}")]
        [Authorize]
        public async Task<IActionResult> DeleteCourse([FromRoute] int id, CancellationToken cancellationToken)
        {
            /*
            // 1) Fetch + Remove
            //var course = await _context.Courses
            //                .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
            //if(course is null)
            //    return NotFound();

            // or
            //2) Stub + Remove
            //A “stub” in this context just means a placeholder entity

            //var course = new Course { Id = id };
            //_context.Courses.Attach(course);

            //_context.Remove(course);
            // Stub + EntityState.Deleted
            // OR//_context.Entry(course).State = EntityState.Deleted;

            //var affected = await _context.SaveChangesAsync(cancellationToken);
            //if (affected == 0)
            //    return NotFound();

            //OR
            // 3) ExecuteDeleteAsync
            //var deleted = await _context.Courses
            //    .Where(x => x.Id == id)
            //    .ExecuteDeleteAsync(cancellationToken);
            //if (deleted == 0)
            //    return NotFound();

            //Soft Delete
            //var course = await _context.Courses
            //                .AsTracking()
            //                .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
            //if (course is null)
            //    return NotFound();

            //course.IsActive = false;
            //await _context.SaveChangesAsync(cancellationToken);

            */
            var instructorId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
            var result = await _courseService.DeleteCourseAsync(id, instructorId, cancellationToken);

            return result.IsSuccess
                ? NoContent()
                : result.ToProblem();
        }
    }
}
