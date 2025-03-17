using FacultyManagementSystemAPI.Models.DTOs.Courses;
using FacultyManagementSystemAPI.Models.Entities;

namespace FacultyManagementSystemAPI.Repositories.Interfaces
{
    public interface ICourseRepository : IGenericRepository<Course>
    {
        Task<IEnumerable<CourseDto>> GetAllWithPreCourseNameAsync();
        Task<CourseDto> GetByIdWithPreCourseNameAsync(int id);
        Task<IEnumerable<CourseDto>> SearchCoursesWithPreCourseNameAsync(string searchTerm);
        Task<IEnumerable<CourseDto>> GetCoursesByDepartmentIdWithPreCourseNameAsync(int departmentId);
        Task<IEnumerable<CourseDto>> GetCoursesByProfessorIdWithPreCourseNameAsync(int professorId);
        Task<IEnumerable<CourseDto>> GetCoursesBySemesterWithPreCourseNameAsync(byte semester);
        Task<IEnumerable<CourseRegistrationStatsDto>> GetCourseRegistrationStatsByCourseOverTimeAsync(int courseId);
        Task<List<CourseDto>> GetCoursesByStudentIdAsync(int studentId);
        Task<bool> CourseExistsAsync(string courseName);
        Task<bool> CourseExistsAsync(int? PreCourseId);
        Task<int> CountAsync();

        Task<int> CountByStatusAsync();
        Task<IEnumerable<string>> GetAllPreRequisiteCoursesAsync();
        Task<IEnumerable<string>> GetAllCoursesStatusesAsync();
        Task<IEnumerable<string>> GetAllCoursesNameAsync();
        Task<IEnumerable<FilterCourseDto>> GetFilteredCoursesAsync(string? courseName, string? departmentName, string? courseStatus);
        Task<CourseStatisticsDto> GetCourseStatisticsAsync(int courseId);

    }
}
