using FacultyManagementSystemAPI.Models.DTOs.Attendance;
using FacultyManagementSystemAPI.Models.Entities;

namespace FacultyManagementSystemAPI.Repositories.Interfaces
{
    public interface IAttendanceRepository : IGenericRepository<Attendance>
    {
        Task<IEnumerable<AttendanceDto>> GetAllAttendancesAsync();
        Task<AttendanceDto> GetAttendanceByIdAsync(int id);
        Task<IEnumerable<AttendanceDto>> GetAttendancesByStudentIdAsync(int studentId);
        Task<IEnumerable<AttendanceDto>> GetAttendancesByClassIdAsync(int classId);

        Task<bool> StudentExistsAsync(int studentId);
        Task<bool> ClassExistsAsync(int classId);
        Task<int> CountAttendanceAsync();
        Task<int> CountNoAttendanceAsync();
    }
}