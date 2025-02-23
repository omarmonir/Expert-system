using FacultyManagementSystemAPI.Models.DTOs.Courses;

namespace FacultyManagementSystemAPI.Services.Interfaces
{
    public interface ICourseService
    {
        Task<CourseDto> GetByIdWithPreCourseNameAsync(int id);
        Task CreateCourseAsync(CreateCourseDto courseDto);
        Task UpdateCourseAsync(int id, UpdateCourseDto updateCourseDto);
        Task DeleteCourseAsync(int id);
        Task<IEnumerable<CourseDto>> GetAllWithPreCourseNameAsync();
        Task<IEnumerable<CourseDto>> SearchCoursesWithPreCourseNameAsync(string searchTerm);
        Task<IEnumerable<CourseDto>> GetCoursesByDepartmentIdWithPreCourseNameAsync(int departmentId);
        Task<IEnumerable<CourseDto>> GetCoursesByProfessorIdWithPreCourseNameAsync(int professorId);
        Task<IEnumerable<CourseDto>> GetCoursesBySemesterWithPreCourseNameAsync(byte semester);

        Task<int> GetCourseCountAsync();
        Task<int> GetCourseCountByStatusAsync();
        Task<IEnumerable<string>> GetAllPreRequisiteCoursesAsync();
    }
}
