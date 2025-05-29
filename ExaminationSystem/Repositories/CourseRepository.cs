using ExaminationSystem.Data;
using ExaminationSystem.Entities;
using Microsoft.EntityFrameworkCore;

namespace ExaminationSystem.Repositories
{
    public class CourseRepository(AppDbContext context)
    {
        private readonly AppDbContext _context = context;

        public IQueryable<Course> GetAll()
        {
            return _context.Courses
                .Where(c => c.IsActive);
        }

        public async Task<Course?> GetByIdAsync(int id)
        {
            var course = await _context.Courses
                .FirstOrDefaultAsync(c => c.Id == id && c.IsActive);

            return course;
        }

        public async Task<Course?> GetByIdWithTrackingAsync(int id)
        {
            var course = await _context.Courses
                .AsTracking()
                .FirstOrDefaultAsync(c => c.Id == id && c.IsActive);

            return course;
        }

        public async Task Update(int id)
        {
            var res = await GetByIdWithTrackingAsync(id);
            if (res is null)
                return;

            await _context.SaveChangesAsync();
        }

        public async Task Delete(int id)
        {
            var res = await GetByIdWithTrackingAsync(id);
            if (res is null)
                return;

            res.IsActive = false;
            await _context.SaveChangesAsync();
        }
    }
}
