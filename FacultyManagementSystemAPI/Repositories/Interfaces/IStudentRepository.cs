using FacultyManagementSystemAPI.Models.DTOs.Courses;
using FacultyManagementSystemAPI.Models.DTOs.Student;
using FacultyManagementSystemAPI.Models.Entities;
using Microsoft.AspNetCore.Mvc;

namespace FacultyManagementSystemAPI.Repositories.Interfaces
{
	public interface IStudentRepository : IGenericRepository<Student>
	{

		// Customer Interfaces
		Task<IEnumerable<StudentDto>> GetAllWithDepartmentNameAsync();
		Task<StudentDto> GetByIdWithDepartmentNameAsync(int id);
		Task<IEnumerable<StudentDto>> GetByNameWithDepartmentNameAsync(string name);
		Task<bool> DepartmentExistsAsync(int departmentId);
		Task<bool> PhoneExistsAsync(string phoneNumber);
		Task<bool> EmailExistsAsync(string email);
		Task<StudentWithGradesDto> GetByIdWithHisGradeAsync(int id);
        Task<IEnumerable<StudentCountDto>> GetStudentCountByDepartmentAsync(int departmentId);
		Task<IEnumerable<CourseStudentCountDto>> GetCourseStudentCount(int courseId);
    }
}
