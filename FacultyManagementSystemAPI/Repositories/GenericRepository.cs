using FacultyManagementSystemAPI.Data;
using Microsoft.EntityFrameworkCore;

namespace FacultyManagementSystemAPI.Repositories
{
	public class GenericRepository<T> : IGenericRepository<T> where T : class
	{
		private readonly AppDbContext _dbContext;
		private readonly DbSet<T> _dbSet;

		public GenericRepository(AppDbContext dbContext)
		{
			_dbContext = dbContext;
			_dbSet = _dbContext.Set<T>();
		}
		public async Task<IEnumerable<T>> GetAllAsync()
		{
			return await _dbSet.AsNoTracking().ToListAsync();
		}

		public async Task<T> GetByIdAsync(int id)
		{
			return await _dbSet.FindAsync(id);
		}

		public async Task AddAsync(T entity)
		{
			await _dbSet.AddAsync(entity);
			await _dbContext.SaveChangesAsync();
		}

		public async Task UpdateAsync(int id, T entity)
		{
			_dbSet.Update(entity);
			await _dbContext.SaveChangesAsync();
		}

		public async Task DeleteAsync(int id)
		{
			var existingEntity = await _dbSet.FindAsync(id);
			if (existingEntity != null)
			{
				_dbSet.Remove(existingEntity);
				await _dbContext.SaveChangesAsync();
			}
		}
	}
}
