using FacultyManagementSystemAPI.Models.DTOs.Enrollment;
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

        Task<IEnumerable<StudentDto>> GetUnenrolledStudentsAsync();

        Task<IEnumerable<StudentDto>> GetUnenrolledStudentsByDepartmentAsync(int departmentId);

        Task<IEnumerable<StudentDto>> GetUnenrolledStudentsBySemesterAsync(byte semester);

        Task<IEnumerable<EnrollmentDto>> GetEnrollmentsByDateRangeAsync(DateTime? minDate, DateTime? maxDate);

        Task<IEnumerable<StudentDto>> GetFilteredStudentsAsync(StudentFilterDto filter);

        Task<IEnumerable<StudentDto>> GetStudentsByDepartmentAndNameAsync(string? departmentName, string? studentName, string? studentStatus);

        Task<IEnumerable<StudentDto>> GetAllByDepartmentIdAsync(int departmentId);
        Task<IEnumerable<StudentDto>> GetStudentsByDepartmentNameAsync(string departmentName);

        Task<IEnumerable<StudentExamGradesDto>> GetStudentsWithExamGradesByCourseIdAsync(int courseId);
        Task<int> GetStudentCountAsync();

        Task<int> CountCanceledEnrolledStudentsAsync();
        Task<int> GetAllEnrollmentStudentsCountAsync();
        Task<int> CountEnrollmentCoursesByStudentIdAsync(int studentId);
        Task<int> CountCompletedCoursesCountStudentIdAsync(int studentId);
        Task<int> CountStudentsByCourseIdAsync(int courseId);
        Task<IEnumerable<string>> GetAllStudentStatusesAsync();
        Task<IEnumerable<string>> GetAllStudentLevelsAsync();
        Task<IEnumerable<string>> GetAllStudentGenderAsync();

        Task UpdateStudentStatusAsync(int studentId, string newStatus);
    }

}
