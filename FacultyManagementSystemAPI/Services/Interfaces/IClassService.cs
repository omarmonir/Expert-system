using FacultyManagementSystemAPI.Models.DTOs.Classes;

namespace FacultyManagementSystemAPI.Services.Interfaces
{
    public interface IClassService
    {
        Task<IEnumerable<ClassDto>> GetAllClassesAsync();
        Task<ClassDto> GetClassByIdAsync(int id);
        Task AddClassAsync(CreateClassDto createClassDto);
        Task UpdateClassAsync(int id, UpdateClassDto updateClassDto);
        Task DeleteClassAsync(int id);
        Task<IEnumerable<ClassDto>> GetClassesByProfessorIdAsync(int professorId);
    }
}
