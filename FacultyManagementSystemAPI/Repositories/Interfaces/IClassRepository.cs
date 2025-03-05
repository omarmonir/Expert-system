using FacultyManagementSystemAPI.Models.DTOs.Classes;
using FacultyManagementSystemAPI.Models.Entities;

namespace FacultyManagementSystemAPI.Repositories.Interfaces
{
    public interface IClassRepository : IGenericRepository<Class>
    {
        Task<Course> GetCourseByIdAsync(int courseId);
        Task<Professor> GetProfessorByNameAsync(string professorName);
        Task<Class> GetClassByProfessorNameAsync(string professorName);
        Task UpdateClassAsync(Class classEntity);
        Task<bool> IsCourseAlreadyAssignedAsync(int courseId, int departmentId);
        Task AssignCourseToProfessorAsync(int courseId, string professorName);
    }
}
