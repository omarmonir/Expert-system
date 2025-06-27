using FacultyManagementSystemAPI.Models.DTOs.Courses;
using FacultyManagementSystemAPI.Models.DTOs.professors;

namespace FacultyManagementSystemAPI.Services.Interfaces
{
    public interface IProfessorService
    {
        Task AddAsync(CreateProfessorDto createProfessorDto);
        Task AddMultipleAsync(CreateProfessorDto createProfessorDto);
        Task UpdateAsync(int id, UpdateProfessorDto updateProfessorDto);
        Task DeleteAsync(int id);
        Task<IEnumerable<ProfessorDto>> GetAllAsync(int pageNumber);
        Task<ProfessorDto?> GetByIdAsync(int id);
        Task<IEnumerable<ProfessorDto>> GetByDepartmentIdAsync(int departmentId);
        Task<IEnumerable<CourseDto>> GetCoursesByProfessorIdAsync(int professorId);
        Task<IEnumerable<string>> GetAllProfessorsNameAsync();
        Task<IEnumerable<ProfessorDto>> GetProfessorsByFiltersAsync(
       string? departmentName,
       string? professorName,
       string? Position);
        Task<IEnumerable<string>> GetAllProfessorsPositionAsync();
    }
}
