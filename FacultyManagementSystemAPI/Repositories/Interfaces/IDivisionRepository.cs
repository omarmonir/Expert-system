using FacultyManagementSystemAPI.Models.Entities;

namespace FacultyManagementSystemAPI.Repositories.Interfaces
{
    public interface IDivisionRepository : IGenericRepository<Division>
    {
        Task<Division?> GetByNameAsync(string name);
        Task<IEnumerable<string>> GetAllDivisionNameAsync();
    }
}
