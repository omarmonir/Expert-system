using FacultyManagementSystemAPI.Models.DTOs.Report;
using FacultyManagementSystemAPI.Repositories.Interfaces;
using FacultyManagementSystemAPI.Services.Interfaces;
using iTextSharp.text.pdf;
using iTextSharp.text;

namespace FacultyManagementSystemAPI.Services.Implementes
{
    //public class ReportService : IReportService
    //{
    //    private readonly IReportRepository _reportRepository;
    //    private readonly ExcelService _excelService;
    //    private readonly PdfService _pdfService;

    //    public ReportService(IReportRepository reportRepository, ExcelService excelService, PdfService pdfService)
    //    {
    //        _reportRepository = reportRepository;
    //        _excelService = excelService;
    //        _pdfService = pdfService;
    //    }
    //    public async Task<byte[]> ExportToExcelAsync<T>(IEnumerable<T> data, string sheetName)
    //    {
    //        return await _excelService.GenerateExcelAsync(data, sheetName);
    //    }

    //    public async Task<byte[]> ExportToPdfAsync<T>(IEnumerable<T> data, string title)
    //    {
    //        return await _pdfService.GeneratePdfAsync(data, title);
    //    }

    //    public async Task<IEnumerable<AcademicWarningDto>> GetAcademicWarningsAsync()
    //    {
    //        return await _reportRepository.GetAcademicWarningsAsync();
    //    }

    //    public Task<IEnumerable<CourseEnrollmentDto>> GetCourseEnrollmentTrendsAsync()
    //    {
    //        throw new NotImplementedException();
    //    }

    //    public Task<IEnumerable<CourseGradeReportDto>> GetCourseGradesAsync()
    //    {
    //        throw new NotImplementedException();
    //    }

    //    public async Task<IEnumerable<ProfessorCoursesDto>> GetProfessorCoursesAsync()
    //    {
    //        return await _reportRepository.GetProfessorCoursesAsync();
    //    }

    //    public Task<IEnumerable<StudentAttendanceDto>> GetStudentAttendanceAsync()
    //    {
    //        throw new NotImplementedException();
    //    }

    //    public Task<IEnumerable<StudentPerCourseDto>> GetStudentsPerCourseAsync()
    //    {
    //        throw new NotImplementedException();
    //    }

    //    public async Task<IEnumerable<StudentPerDepartmentDto>> GetStudentsPerDepartmentAsync()
    //    {
    //        return await _reportRepository.GetStudentsPerDepartmentAsync();
    //    }
    //}
  
}
