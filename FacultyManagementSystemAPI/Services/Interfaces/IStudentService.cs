using FacultyManagementSystemAPI.Models.DTOs.Courses;
using FacultyManagementSystemAPI.Models.DTOs.Student;
namespace FacultyManagementSystemAPI.Services.Interfaces
{
	public interface IStudentService
	{
		//Task<IEnumerable<StudentDto>> GetAllAsync();
		//Task<StudentDto> GetByIdAsync(int id);
		Task AddAsync(CreateStudentDto createStudentDto);
		Task UpdateAsync(int id, UpdateStudentDto updateStudentDto);
		Task DeleteAsync(int id);

		// Customer Interfaces
		Task<IEnumerable<StudentDto>> GetAllWithDepartmentNameAsync();
		Task<StudentDto> GetByIdWithDepartmentNameAsync(int id);
		Task<IEnumerable<StudentDto>> GetByNameWithDepartmentNameAsync(string name);
		Task<StudentWithGradesDto> GetByIdWithHisGradeAsync(int id);
        Task<IEnumerable<StudentCountDto>> GetStudentCountByDepartmentAsync(int departmentId);
        public Task<IEnumerable<CourseStudentCountDto>> GetCourseStudentCount(int courseId);
    }

}
