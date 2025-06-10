using FacultyManagementSystemAPI.Data;
using FacultyManagementSystemAPI.Models.Entities;
using FacultyManagementSystemAPI.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace FacultyManagementSystemAPI.Repositories.Implementes
{
    public class DivisionRepository(AppDbContext dbContext) : GenericRepository<Division>(dbContext), IDivisionRepository
    {
        private readonly AppDbContext _dbContext = dbContext;
        public async Task<Division?> GetByNameAsync(string name)
        {
            return await _dbContext.Divisions
                .FirstOrDefaultAsync(d => d.Name.ToLower() == name.ToLower());
        }
        public async Task<IEnumerable<string>> GetAllDivisionNameAsync()
        {
            var names = await _dbContext.Divisions
                .Select(d => d.Name)
                .ToListAsync();

            return names;
        }

    }
}
