using ExaminationSystem.Data;
using ExaminationSystem.Models;
using ExaminationSystem.Repositories;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ExaminationSystem.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CoursesController(GeneralRepository<Course> generalRepository) : ControllerBase
    {
        private readonly GeneralRepository<Course> _generalRepository = generalRepository;

        [HttpGet("")]
        public IActionResult GetAll()
        {
            var courses = _generalRepository.GetAll();

            return Ok(courses);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var course = await _generalRepository.GetByIdAsync(id);
            if(course is null)
                return NotFound();

            return Ok(course);
        }

        [HttpPost("")]
        public async Task<bool> AddCourse(Course course)
        {
            //await _context.Courses.AddAsync(course);
            //await _context.SaveChangesAsync();

            return true;
        }

        [HttpPut("")]
        public async Task<bool> UpdateCourse(Course course)
        {
            //await _generalRepository.UpdateIncludeAsync(course, nameof(course.Name), nameof(course.Hours));
            //or
            await _generalRepository.UpdateAsync(c => c.Id == course.Id,
                s => s
                    .SetProperty(c => c.Name, "new name")
                    .SetProperty(c => c.Hours, 50));


            return true;
        }


        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCourse(int id, CancellationToken cancellationToken)
        {
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

            return NoContent();
        }
    }
}
