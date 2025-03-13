using FacultyManagementSystemAPI.Models.DTOs.Report;

namespace FacultyManagementSystemAPI.Services.Interfaces
{
    public interface IReportService
    {
        Task<IEnumerable<StudentPerDepartmentDto>> GetStudentsPerDepartmentAsync();
        Task<IEnumerable<AcademicWarningDto>> GetAcademicWarningsAsync();
        Task<IEnumerable<StudentPerCourseDto>> GetStudentsPerCourseAsync();
        Task<IEnumerable<CourseEnrollmentDto>> GetCourseEnrollmentTrendsAsync();
        Task<IEnumerable<CourseGradeReportDto>> GetCourseGradesAsync();
        Task<IEnumerable<ProfessorCoursesDto>> GetProfessorCoursesAsync();
        Task<IEnumerable<StudentAttendanceDto>> GetStudentAttendanceAsync();

        Task<byte[]> ExportToExcelAsync<T>(IEnumerable<T> data, string sheetName);
        Task<byte[]> ExportToPdfAsync<T>(IEnumerable<T> data, string title);
    }
}
