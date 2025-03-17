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
        Task<IEnumerable<CourseRegistrationStatsDto>> GetCourseRegistrationStatsByCourseOverTimeAsync(int courseId);
        Task<IEnumerable<CourseDto>> GetCoursesByStudentIdAsync(int studentId);

        Task<int> GetCourseCountAsync();
        Task<int> GetCourseCountByStatusAsync();
        Task<IEnumerable<string>> GetAllPreRequisiteCoursesAsync();
        Task DeleteAsync(int id);
        Task<IEnumerable<string>> GetAllCoursesStatusesAsync();
        Task<IEnumerable<string>> GetAllCoursesNameAsync();
        Task<IEnumerable<FilterCourseDto>> GetFilteredCoursesAsync(string? courseName, string? departmentName, string? courseStatus);
        Task<CourseStatisticsDto> GetCourseStatisticsAsync(int courseId);
    }
}
