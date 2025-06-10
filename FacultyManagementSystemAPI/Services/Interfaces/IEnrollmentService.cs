using FacultyManagementSystemAPI.Models.DTOs;
using FacultyManagementSystemAPI.Models.DTOs.Enrollment;

namespace FacultyManagementSystemAPI.Services.Interfaces
{
    public interface IEnrollmentService
    {

        Task AddAsync(CreateEnrollmentDto enrollmentDto);
        Task<IEnumerable<EnrollmentDto>> GetAllIncludeStudentNameCourseNameAsync(int pageNumber);
        //Task<PaginatedResult<EnrollmentDto>> GetAllIncludeStudentNameCourseNameAsync(int pageNumber);
        Task<EnrollmentDto> GetByIdIncludeStudentNameCourseNameAsync(int id);
        Task<IEnumerable<EnrollmentDto>> GetBySemesterAsync(string name);
        Task<IEnumerable<EnrollmentDto>> GetByStudentIdAsync(int studentId);
        Task<IEnumerable<EnrollmentDto>> GetByCourseIdAsync(int courseId);
        Task DeleteAsync(int id);
        Task<IEnumerable<string>> GetAllEnrollmentsStatusesAsync();
        Task<int> GetEnrollmentCountAsync();
        Task<int> GetCanceledEnrollmentCountAsync();
        Task<int> GetAllEnrollmentStudentsCountAsync();
        Task<int> GetAllWaitEnrollmentStudentsCountAsync();
        Task<double> GetSuccessPercentageAsync();
        Task<IEnumerable<EnrollmentDto>> GetFilteredEnrollmentsAsync(string? studentName, string? courseName, string? enrollmentStatus, string? semester);

        Task UpdateStudentGradeAsync(int studentId, int courseId, decimal newGrade);
        Task UpdateStudentExam1GradeAsync(int studentId, int courseId, decimal newGrade);
        Task UpdateStudentExam2GradeAsync(int studentId, int courseId, decimal newGrade);
        Task<IEnumerable<string>> GetAllEnrollmentsSemsterAsync();
    }
}