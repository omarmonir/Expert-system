using FacultyManagementSystemAPI.Models.DTOs.Attendance;

namespace FacultyManagementSystemAPI.Services.Interfaces
{
    public interface IAttendanceService
    {
        Task<IEnumerable<AttendanceDto>> GetAllAttendancesAsync(int pageNumber);
        Task<AttendanceDto> GetAttendanceByIdAsync(int id);
        Task AddAttendanceAsync(CreateAttendanceDto createAttendanceDto, int professorId);
        Task UpdateAttendanceAsync(int id, UpdateAttendanceDto dto, int professorId);
        Task DeleteAttendanceAsync(int id);
        Task<IEnumerable<AttendanceDto>> GetAttendancesByStudentIdAsync(int studentId, int pageNumber);
        Task<IEnumerable<AttendanceDto>> GetAttendancesByClassIdAsync(int classId, int pageNumber);

        Task<int> CountAttendanceAsync();
        Task<int> CountNoAttendanceAsync();
        Task<double> GetSuccessPercentageAsync();
    }
}