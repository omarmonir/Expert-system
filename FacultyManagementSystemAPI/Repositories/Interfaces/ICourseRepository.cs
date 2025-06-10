using FacultyManagementSystemAPI.Models.DTOs.Courses;
using FacultyManagementSystemAPI.Models.Entities;

namespace FacultyManagementSystemAPI.Repositories.Interfaces
{
    public interface ICourseRepository : IGenericRepository<Course>
    {
        Task<IEnumerable<CourseDto>> GetAllWithPreCourseNameAsync(int pageNumber);
        Task<CourseDto> GetByIdWithPreCourseNameAsync(int id);
        //Task<IEnumerable<CourseDto>> SearchCoursesWithPreCourseNameAsync(string searchTerm);
        //Task<IEnumerable<CourseDto>> GetCoursesByDepartmentIdWithPreCourseNameAsync(int departmentId);
        Task<IEnumerable<CourseDto>> GetCoursesByProfessorIdWithPreCourseNameAsync(int professorId);
        //Task<IEnumerable<CourseDto>> GetCoursesBySemesterWithPreCourseNameAsync(byte semester);
        //Task<IEnumerable<CourseRegistrationStatsDto>> GetCourseRegistrationStatsByCourseOverTimeAsync(int courseId);
        Task<List<CourseDto>> GetCoursesByStudentIdAsync(int studentId);
        Task<bool> CourseExistsAsync(string courseName);
        Task<Course> GetByIdWithEnrollmentsAsync(int id);

        Task<bool> CourseExistsAsync(int? PreCourseId);
        Task<int> CountAsync();
        //Task<int> CountActiveCourseAsync();
        //Task<int> CountByStatusAsync();
        //Task<IEnumerable<string>> GetAllPreRequisiteCoursesAsync();
        //Task<IEnumerable<string>> GetAllCoursesStatusesAsync();
        Task<IEnumerable<string>> GetAllCoursesNameAsync();
        Task<IEnumerable<FilterCourseDto>> GetFilteredCoursesAsync(string? courseName, string? departmentName, string? courseStatus, string? divisionName);
        Task<CourseStatisticsDto> GetCourseStatisticsAsync(int courseId);
        //Task<IEnumerable<CourseDto>> SearchCoursesWithCourseNameAndStatusAsync(string searchTerm, string status);
        Task<Department> GetDepartmentByNameAsync(string name);
        Task<List<Division>> GetDivisionsByNamesAsync(List<string> names);
        Task<Course> GetCoursesByNamesAsync(string names);
        Task AddCourseAsync(Course course);
        Task SaveChangesAsync();
        Task<List<Course>> GetCoursesByNamesPreAsync(List<string> names);
    }
}
