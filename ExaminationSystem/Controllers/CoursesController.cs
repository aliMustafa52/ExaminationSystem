using ExaminationSystem.Data;
using ExaminationSystem.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ExaminationSystem.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CoursesController(AppDbContext context) : ControllerBase
    {
        private readonly AppDbContext _context = context;

        [HttpGet("")]
        public async Task<IActionResult> GetAll()
        {
            var courses = await _context.Courses
                            .AsNoTracking()
                            .ToListAsync();

            return Ok(courses);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var course = await _context.Courses
                            .AsNoTracking()
                            .FirstOrDefaultAsync(x => x.Id == id);

            return Ok(course);
        }

        [HttpPost("")]
        public async Task<bool> AddCourse(Course course)
        {
            await _context.Courses.AddAsync(course);
            await _context.SaveChangesAsync();

            return true;
        }

        [HttpDelete("{id}")]
        public async Task<bool> DeleteCourse(int id)
        {
            var course = await _context.Courses
                            .FirstOrDefaultAsync(x => x.Id == id);
            if(course is null)
                return false;

            _context.Remove(course);
            await _context.SaveChangesAsync();

            return true;
        }
    }
}
