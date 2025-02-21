using FacultyManagementSystemAPI.Models.DTOs.Student;
namespace FacultyManagementSystemAPI.Services.Interfaces
{
	public interface IStudentService
	{
		Task<IEnumerable<StudentDto>> GetAllAsync();
		//Task<StudentDto> GetByIdAsync(int id);
		Task AddAsync(CreateStudentDto createStudentDto);
		Task UpdateAsync(int id, UpdateStudentDto updateStudentDto);
		Task DeleteAsync(int id);

		// Customer Interfaces
		Task<IEnumerable<StudentDto>> GetAllWithDepartmentNameAsync();
		Task<StudentDto> GetByIdWithDepartmentNameAsync(int id);
	}
}
