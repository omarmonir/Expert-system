using FacultyManagementSystemAPI.Models.DTOs.Classes;
using FacultyManagementSystemAPI.Models.DTOs.Report;

namespace FacultyManagementSystemAPI.Services.Interfaces
{
    public interface IReportService
    {
        //Task<IEnumerable<StudentPerDepartmentDto>> GetStudentsPerDepartmentAsync();
        Task<IEnumerable<AcademicWarningDto>> GetAcademicWarningsAsync();
        Task<IEnumerable<StudentPerCourseDto>> GetStudentsPerCourseAsync();
        Task<IEnumerable<CourseEnrollmentDto>> GetCourseEnrollmentTrendsAsync();
        Task<IEnumerable<CourseGradeReportDto>> GetCourseGradesAsync();
        Task<IEnumerable<ProfessorCoursesDto>> GetProfessorCoursesAsync();
        Task<IEnumerable<StudentAttendanceDto>> GetStudentAttendanceAsync();
        Task<IEnumerable<FilterDto>> GetFilteredCoursesAsync
            (string? courseName, string? departmentName, string? courseStatus, string? divisionName);

        Task<byte[]> ExportToExcelAsync<T>(IEnumerable<T> data, string sheetName);
        Task<byte[]> ExportToPdfAsync<T>(IEnumerable<T> data, string title);
        Task<byte[]> GenerateProfessorSchedulePdfAsync(IEnumerable<ClassDto> classes, string professorName);
        Task<byte[]> GenerateStudentSchedulePdfAsync(IEnumerable<ClassDto> classes, string StudentName);
        Task<byte[]> GenerateAdminClassesPdfAsync(IEnumerable<ClassDto> classes, string? filterInfo = null);
    }
}
