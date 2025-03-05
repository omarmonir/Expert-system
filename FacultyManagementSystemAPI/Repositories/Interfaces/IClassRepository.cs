using FacultyManagementSystemAPI.Models.DTOs.Classes;
using FacultyManagementSystemAPI.Models.Entities;

namespace FacultyManagementSystemAPI.Repositories.Interfaces
{
    public interface IClassRepository : IGenericRepository<Class>
    {
        Task<IEnumerable<ClassDto>> GetAllClassesAsync();
        Task<IEnumerable<ClassDto>> GetClassesByProfessorIdAsync(int professorId);

    }
}
