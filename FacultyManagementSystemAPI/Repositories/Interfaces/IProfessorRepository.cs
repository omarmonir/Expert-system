using FacultyManagementSystemAPI.Models.DTOs.Courses;
using FacultyManagementSystemAPI.Models.DTOs.professors;
using FacultyManagementSystemAPI.Models.Entities;

namespace FacultyManagementSystemAPI.Repositories.Interfaces
{
    public interface IProfessorRepository : IGenericRepository<Professor>
    {
        Task<IEnumerable<ProfessorDto>> GetAllProfessorsAsync();
        Task<ProfessorDto?> GetProfessorByIdAsync(int id);
        Task<Department?> GetDepartmentByNameAsync(string name);
        Task<IEnumerable<ProfessorDto>> GetByDepartmentIdAsync(int departmentId);
        Task<IEnumerable<CourseDto>> GetCoursesByProfessorIdAsync(int professorId);
        Task<bool> ProfessorExistsAsync(string professorName);
        Task<IEnumerable<string>> GetAllProfessorsNameAsync();
    }
}
