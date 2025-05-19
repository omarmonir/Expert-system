using FacultyManagementSystemAPI.Models.Entities;

namespace FacultyManagementSystemAPI.Repositories.Interfaces
{
    public interface IDepartmentRepository : IGenericRepository<Department>
    {
        Task<IEnumerable<string>> GetAllDepartmentNameAsync();
        Task<int?> GetIdOfDepartmentByNameAsync(string DepartmentName);
    }
}