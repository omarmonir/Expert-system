using FacultyManagementSystemAPI.Services.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace FacultyManagementSystemAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ReportsController(IReportService reportService) : ControllerBase
    {
        private readonly IReportService _reportService = reportService;

        [HttpGet("students-per-department/excel")]
        public async Task<IActionResult> ExportStudentsPerDepartmentToExcel()
        {
            var data = await _reportService.GetStudentsPerDepartmentAsync();
            var fileBytes = await _reportService.ExportToExcelAsync(data, "StudentsPerDepartment");
            return File(fileBytes, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "StudentsPerDepartment.xlsx");
        }

        [HttpGet("AcademicWarning/excel")]
        public async Task<IActionResult> ExportAcademicWarningToExcel()
        {
            var data = await _reportService.GetAcademicWarningsAsync();
            var fileBytes = await _reportService.ExportToExcelAsync(data, "AcademicWarnings");
            return File(fileBytes, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "AcademicWarnings.xlsx");
        }
        
        [HttpGet("ProfessorCourses/excel")]
        public async Task<IActionResult> ExportProfessorCoursesToExcel()
        {
            var data = await _reportService.GetProfessorCoursesAsync();
            var fileBytes = await _reportService.ExportToExcelAsync(data, "ProfessorCourses");
            return File(fileBytes, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "ProfessorCourses.xlsx");
        }

        [HttpGet("ProfessorCourses/pdf")]
        public async Task<IActionResult> ExportProfessorCoursesToPdf()
        {
            var data = await _reportService.GetProfessorCoursesAsync();
            var fileBytes = await _reportService.ExportToPdfAsync(data, "Professor Courses Report");
            return File(fileBytes, "application/pdf", "ProfessorCourses.pdf");
        } 
        
        [HttpGet("students-per-department/pdf")]
        public async Task<IActionResult> ExportStudentsPerDepartmentToPdf()
        {
            var data = await _reportService.GetStudentsPerDepartmentAsync();
            var fileBytes = await _reportService.ExportToPdfAsync(data, "Students Per Department Report");
            return File(fileBytes, "application/pdf", "StudentsPerDepartment.pdf");
        }
        
        [HttpGet("AcademicWarnings/pdf")]
        public async Task<IActionResult> ExportAcademicWarningsToPdf()
        {
            var data = await _reportService.GetAcademicWarningsAsync();
            var fileBytes = await _reportService.ExportToPdfAsync(data, "Academic Warnings Report");
            return File(fileBytes, "application/pdf", "AcademicWarnings.pdf");
        }
    }
}
