using ExaminationSystem.Data;
using ExaminationSystem.Models;
using Microsoft.EntityFrameworkCore;

namespace ExaminationSystem.Repositories
{
    public class GeneralRepository<T> where T : BaseModel
    {
        private readonly AppDbContext _context;
        private readonly DbSet<T> _dbSet;

        public GeneralRepository(AppDbContext context)
        {
            _context = context;
            _dbSet = _context.Set<T>();
        }

        public IQueryable<T> GetAll()
        {
            return _dbSet
                .Where(c => c.IsActive);
        }

        public async Task<T?> GetByIdAsync(int id)
        {
            var course = await _dbSet
                .FirstOrDefaultAsync(c => c.Id == id && c.IsActive);

            return course;
        }

        public async Task<T?> GetByIdWithTrackingAsync(int id)
        {
            var course = await _dbSet
                .AsTracking()
                .FirstOrDefaultAsync(c => c.Id == id && c.IsActive);

            return course;
        }

        public async Task<bool> AddAsync(T t)
        {
            await _dbSet.AddAsync(t);
            await _context.SaveChangesAsync();

            return true;
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
