using FacultyManagementSystemAPI.Models.DTOs.Classes;

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
        Task<IEnumerable<ClassDto>> GetAllClassesWithProfNameAndCourseNameAsync(
                string? divisionName = null,
                 byte? semester = null);

    }
}
