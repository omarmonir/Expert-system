using FacultyManagementSystemAPI.Models.DTOs.Courses;
using FacultyManagementSystemAPI.Models.DTOs.professors;
using FacultyManagementSystemAPI.Models.DTOs.Student;

namespace FacultyManagementSystemAPI.Services.Interfaces
{
    public interface IProfessorService
    {
        Task AddAsync(CreateProfessorDto createProfessorDto);
        Task UpdateAsync(int id, UpdateProfessorDto updateProfessorDto);
        Task DeleteAsync(int id);
        Task<IEnumerable<ProfessorDto>> GetAllAsync();
        Task<ProfessorDto?> GetByIdAsync(int id);
        Task<IEnumerable<ProfessorDto>> GetByDepartmentIdAsync(int departmentId);
        Task<IEnumerable<CourseDto>> GetCoursesByProfessorIdAsync(int professorId);
        Task<IEnumerable<string>> GetAllProfessorsNameAsync();
    }
}
