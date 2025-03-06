using FacultyManagementSystemAPI.Models.DTOs.Enrollment;
using FacultyManagementSystemAPI.Models.DTOs.Student;
using FacultyManagementSystemAPI.Models.Entities;

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
        Task<IEnumerable<StudentDto>> GetUnenrolledStudentsAsync();

        Task<IEnumerable<StudentDto>> GetUnenrolledStudentsByDepartmentAsync(int departmentId);

        Task<IEnumerable<StudentDto>> GetUnenrolledStudentsBySemesterAsync(byte semester);

        Task<IEnumerable<EnrollmentDto>> GetEnrollmentsByDateRangeAsync(DateTime? minDate, DateTime? maxDate);

        Task<IEnumerable<StudentExamGradesDto>> GetStudentsWithExamGradesByCourseIdAsync(int courseId);

        Task<int> CountAsync();
        // Filtering

        Task<IEnumerable<StudentDto>> GetFilteredStudentsAsync(StudentFilterDto filter);
        Task<IEnumerable<StudentDto>> GetStudentsByDepartmentAndNameAsync(string? departmentName, string? studentName, string? studentStatus);
        Task<IEnumerable<StudentDto>> GetAllByDepartmentIdAsync(int departmentId);
        Task<IEnumerable<StudentDto>> GetStudentsByDepartmentNameAsync(string departmentName);
        Task<int> CountCanceledEnrolledStudentsAsync();

        Task<int> CountEnrollmentCoursesByStudentIdAsync(int studentId);
        Task<int> CountCompletedCoursesCountStudentIdAsync(int studentId);
        Task<int> GetAllEnrollmentStudentsCountAsync();
        Task<IEnumerable<string>> GetAllStudentStatusesAsync();
        Task<IEnumerable<string>> GetAllStudentLevelsAsync();
        Task<IEnumerable<string>> GetAllStudentGenderAsync();

        Task UpdateStudentStatusAsync(int studentId, string newStatus);
    }
}
