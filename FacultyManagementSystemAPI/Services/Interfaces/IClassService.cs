using FacultyManagementSystemAPI.Models.DTOs.Classes;

namespace FacultyManagementSystemAPI.Services.Interfaces
{
    public interface IClassService
    {
        Task AssignCourseToProfessorAsync(int courseId, string professorName);
        Task<IEnumerable<ClassDto>> GetAllClassesWithProfNameAndCourseNameAsync();
        Task<ClassDto> GetClassByIdAsync(int id);
        Task CreateClassAsync(CreateClassDto createClassDto);
        Task UpdateClassAsync(int id, UpdateClassDto updateClassDto);
        Task DeleteClassAsync(int id);
        Task<int> GetClassCountAsync();
        Task<IEnumerable<string>> GetAllLocationsNameAsync();
    }
}
