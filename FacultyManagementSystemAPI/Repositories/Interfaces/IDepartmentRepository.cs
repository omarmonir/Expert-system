using FacultyManagementSystemAPI.Models.Entities;

namespace FacultyManagementSystemAPI.Repositories.Interfaces
{
    public interface IDepartmentRepository : IGenericRepository<Department>
    {
        Task<IEnumerable<string>> GetAllDepartmentNameAsync();
        Task UpdateProfessorCountAsync(int DepartmentId, int professorCount);
        Task UpdateHeadOfDepartmentAsync(int DepartmentId, string headOfDepartment);
        Task<int?> GetIdOfDepartmentByNameAsync(string DepartmentName);
    }
}