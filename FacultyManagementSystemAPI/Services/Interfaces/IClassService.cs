using FacultyManagementSystemAPI.Models.DTOs.Classes;
using FacultyManagementSystemAPI.Models.Entities;

namespace FacultyManagementSystemAPI.Services.Interfaces
{
    public interface IClassService
    {
        Task AssignCourseToProfessorAsync(int courseId, string professorName);
        Task<IEnumerable<ClassDto>> GetAllClassesWithProfNameAndCourseNameAsync(int pageNumber);
        Task<ClassDto> GetClassByIdAsync(int id);
        Task CreateClassByNameAsync(CreateClassByNameDto dto);
        Task UpdateClassAsync(int id, UpdateClassDto updateClassDto);
        Task DeleteClassAsync(int id);
        Task<int> GetClassCountAsync();
        Task<IEnumerable<string>> GetAllLocationsNameAsync();
        Task<IEnumerable<ClassDto>> GetAllClassesWithProfNameAndCourseNameAsyncOptimized(
       string? divisionName = null,
       string? level = null);
        Task<IEnumerable<ClassDto>> GetProfessorClassesAsync(int professorId);
        Task<IEnumerable<ClassDto>> GetStudentClassesAsync(int studentId);
    }
}
