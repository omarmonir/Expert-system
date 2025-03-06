using FacultyManagementSystemAPI.Models.DTOs.Attendance;

namespace FacultyManagementSystemAPI.Services.Interfaces
{
    public interface IAttendanceService
    {
        Task<IEnumerable<AttendanceDto>> GetAllAttendancesAsync();
        Task<AttendanceDto> GetAttendanceByIdAsync(int id);
        Task AddAttendanceAsync(CreateAttendanceDto createAttendanceDto);
        Task UpdateAttendanceAsync(int id, UpdateAttendanceDto attendance);
        Task DeleteAttendanceAsync(int id);
        Task<IEnumerable<AttendanceDto>> GetAttendancesByStudentIdAsync(int studentId);
        Task<IEnumerable<AttendanceDto>> GetAttendancesByClassIdAsync(int classId);

        Task<int> CountAttendanceAsync();
        Task<int> CountNoAttendanceAsync();
        Task<double> GetSuccessPercentageAsync();
    }
}