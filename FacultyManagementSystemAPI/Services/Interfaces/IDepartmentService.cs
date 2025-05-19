using FacultyManagementSystemAPI.Models.DTOs.Department;

namespace FacultyManagementSystemAPI.Services.Interfaces
{
    public interface IDepartmentService
    {
        Task<IEnumerable<DepartmentDto>> GetAllAsync();
        Task<DepartmentDto> GetByIdAsync(int id);
        Task<IEnumerable<string>> GetDepartmentNameAsync();
        Task<int?> GetIdOfDepartmentByNameAsync(string DepartmentName);
    }
}