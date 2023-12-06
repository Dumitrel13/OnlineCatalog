using Microsoft.EntityFrameworkCore;
using OnlineCatalog.Data;
using OnlineCatalog.Repository.Interfaces;

namespace OnlineCatalog.Repository
{
	public class BaseRepository<T> : IBaseRepository<T> where T : class
	{
        protected readonly AppDbContext _context;
        protected readonly DbSet<T> _dbSet;

        public BaseRepository(AppDbContext context)
        {
            _context = context;
            _dbSet = context.Set<T>();
        }

        public async Task AddAsync(T entity)
		{
			await _dbSet.AddAsync(entity);
		}
        public Task UpdateAsync(T entity)
        {
			_context.Update(entity);
			return Task.CompletedTask;
        }
        public Task DeleteAsync(T entity)
		{
			_dbSet.Remove(entity);
			return Task.CompletedTask;
		}
        public async Task<ICollection<T>> GetAllAsync()
		{
			return await _dbSet.ToListAsync();
		}
	}
}
