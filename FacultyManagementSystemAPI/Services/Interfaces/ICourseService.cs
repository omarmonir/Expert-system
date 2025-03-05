using FacultyManagementSystemAPI.Models.DTOs.Courses;
using FacultyManagementSystemAPI.Models.Entities;

namespace FacultyManagementSystemAPI.Services.Interfaces
{
	public interface ICourseService
	{
		Task<CourseDto> GetByIdWithPreCourseNameAsync(int id);
		Task CreateCourseAsync(CreateCourseDto courseDto);
		Task UpdateCourseAsync(int id, UpdateCourseDto upfateCourseDto);
		Task DeleteCourseAsync(int id);
		Task<IEnumerable<CourseDto>> GetAllWithPreCourseNameAsync();
		Task<IEnumerable<CourseDto>> SearchCoursesWithPreCourseNameAsync(string searchTerm);
		Task<IEnumerable<CourseDto>> GetCoursesByDepartmentIdWithPreCourseNameAsync(int departmentId);
		Task<IEnumerable<CourseDto>> GetCoursesByProfessorIdWithPreCourseNameAsync(int professorId);
		Task<IEnumerable<CourseDto>> GetCoursesBySemesterWithPreCourseNameAsync(byte semester);
        Task<IEnumerable<CourseRegistrationStatsDto>> GetCourseRegistrationStatsByCourseOverTimeAsync(int courseId);
        Task<IEnumerable<CourseDto>> GetCoursesByStudentIdAsync(int studentId);

    }
}
