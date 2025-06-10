using FacultyManagementSystemAPI.Models.DTOs.Enrollment;
using FacultyManagementSystemAPI.Models.Entities;

namespace FacultyManagementSystemAPI.Repositories.Interfaces
{
    public interface IEnrollmentRepository : IGenericRepository<Enrollment>
    {
        //Task<IEnumerable<EnrollmentDto>> GetAllIncludeStudentNameCourseNameAsync();
        Task<IEnumerable<EnrollmentDto>> GetAllIncludeStudentNameCourseNameAsync(int pageNumber);
        Task<EnrollmentDto> GetByIdIncludeStudentNameCourseNameAsync(int id);
        Task<IEnumerable<EnrollmentDto>> GetBySemesterAsync(string name);
        Task<IEnumerable<EnrollmentDto>> GetByStudentIdAsync(int studentId);
        Task<IEnumerable<EnrollmentDto>> GetByCourseIdAsync(int courseId);


        Task<Enrollment> GetByStudentAndCourseIdAsync(int studentId, int courseId);
        Task UpdateAsync(Enrollment enrollment);
        Task<int> CountAsync();
        Task<int> CountDeletedAsync();
        Task<int> GetCompletedEnrollmentCountAsync();
        Task<IEnumerable<EnrollmentDto>> GetFilteredEnrollmentsAsync(string? studentName, string? courseName, string? enrollmentStatus, string? semester);
        Task<int> GetAllEnrollmentStudentsCountAsync();
        Task<int> GetAllWaitEnrollmentStudentsCountAsync();
        Task<IEnumerable<string>> GetAllEnrollmentsStatusesAsync();
        Task<IEnumerable<string>> GetAllEnrollmentsSemsterAsync();
        Task<Course?> GetCourseByIdAsync(int id);
        Task<Student?> GetStudentByIdAsync(int id);
        Task<bool> ExistsAsync(int studentId, int courseId);
    }

}