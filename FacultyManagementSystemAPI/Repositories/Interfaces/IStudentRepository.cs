using FacultyManagementSystemAPI.Models.DTOs.Student;
using FacultyManagementSystemAPI.Models.Entities;

namespace FacultyManagementSystemAPI.Repositories.Interfaces
{
	public interface IStudentRepository : IGenericRepository<Student>
	{
		// Customer Interfaces
		Task<IEnumerable<StudentDto>> GetAllWithDepartmentNameAsync();
		Task<StudentDto> GetByIdWithDepartmentNameAsync(int id);
		Task<bool> DepartmentExistsAsync(int departmentId);
		Task<bool> PhoneExistsAsync(string phoneNumber);
		Task<bool> EmailExistsAsync(string email);
	}
}
