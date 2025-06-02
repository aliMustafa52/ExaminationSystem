using Azure;
using ExaminationSystem.Abstractions;
using ExaminationSystem.Contracts.Courses;
using ExaminationSystem.Services.CoursesService;
using Microsoft.AspNetCore.Mvc;

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
        public async Task<ActionResult<CourseResponse>> AddCourse([FromBody] CourseRequest request)
        {
            var result = await _courseService.AddCourseAsync(request);

            return result.IsSuccess
            ? CreatedAtAction(nameof(GetById), new { id = result.Value.Id }, result.Value)
            : result.ToProblem();
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateCourse([FromRoute] int id,[FromBody] CourseRequest request)
        {
            var result = await _courseService.UpdateCourseAsync(id, request);

            return result.IsSuccess
                ? NoContent()
                : result.ToProblem();
        }


        [HttpDelete("{id}")]
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

            var result = await _courseService.DeleteCourseAsync(id, cancellationToken);

            return result.IsSuccess
                ? NoContent()
                : result.ToProblem();
        }
    }
}
