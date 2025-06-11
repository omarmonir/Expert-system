using FacultyManagementSystemAPI.Models.DTOs.Report;

namespace FacultyManagementSystemAPI.Repositories.Interfaces
{
    public interface IReportRepository
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
    }
}
