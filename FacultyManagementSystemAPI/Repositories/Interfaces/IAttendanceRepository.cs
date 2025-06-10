using FacultyManagementSystemAPI.Models.DTOs.Attendance;
using FacultyManagementSystemAPI.Models.Entities;

namespace FacultyManagementSystemAPI.Repositories.Interfaces
{
    public interface IAttendanceRepository : IGenericRepository<Attendance>
    {
        Task<IEnumerable<AttendanceDto>> GetAllAttendancesAsync(int pageNumber);
        Task<AttendanceDto> GetAttendanceByIdAsync(int id);
        Task<IEnumerable<AttendanceDto>> GetAttendancesByStudentIdAsync(int studentId, int pageNumber);
        Task<IEnumerable<AttendanceDto>> GetAttendancesByClassIdAsync(int classId, int pageNumber);

        Task<bool> StudentExistsAsync(int studentId);
        Task<bool> ClassExistsAsync(int classId);
        Task<int> CountAttendanceAsync();
        Task<int> CountNoAttendanceAsync();
        Task<Course> GetCourseByNameAsync(string name);
        Task<Class> GetClassByProfessorAndCourseAsync(int professorId, int courseId);
    }
}