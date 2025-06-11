using FacultyManagementSystemAPI.Services.Implementes;
using FacultyManagementSystemAPI.Services.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using static PdfService;

namespace FacultyManagementSystemAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ReportsController(IReportService reportService, IStudentService studentService, ICourseService courseService, IClassService classService) : ControllerBase
    {
        private readonly IReportService _reportService = reportService;
        private readonly IStudentService _studentService = studentService;
        private readonly ICourseService _courseService = courseService;
        private readonly IClassService _classService = classService;
        private readonly DynamicTitleGenerator _titleGenerator = new DynamicTitleGenerator();


        //    [HttpGet("students-per-department/excel")]
        //    public async Task<IActionResult> ExportStudentsPerDepartmentToExcel()
        //    {
        //        var data = await _reportService.GetStudentsPerDepartmentAsync();
        //        var fileBytes = await _reportService.ExportToExcelAsync(data, "StudentsPerDepartment");
        //        return File(fileBytes, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "StudentsPerDepartment.xlsx");
        //    }

        //    [HttpGet("AcademicWarning/excel")]
        //    public async Task<IActionResult> ExportAcademicWarningToExcel()
        //    {
        //        var data = await _reportService.GetAcademicWarningsAsync();
        //        var fileBytes = await _reportService.ExportToExcelAsync(data, "AcademicWarnings");
        //        return File(fileBytes, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "AcademicWarnings.xlsx");
        //    }

        //    [HttpGet("AllStudents/excel")]
        //    public async Task<IActionResult> ExportAllStudentsToExcel()
        //    {
        //        var data = await _studentService.GetAllWithDepartmentNameAsync();
        //        var fileBytes = await _reportService.ExportToExcelAsync(data, "Students Data");
        //        return File(fileBytes, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "Students Data.xlsx");
        //    }
        //    [HttpGet("ProfessorCourses/excel")]
        //    public async Task<IActionResult> ExportProfessorCoursesToExcel()
        //    {
        //        var data = await _reportService.GetProfessorCoursesAsync();
        //        var fileBytes = await _reportService.ExportToExcelAsync(data, "ProfessorCourses");
        //        return File(fileBytes, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "ProfessorCourses.xlsx");
        //    }

        //    [HttpGet("AllStudents/pdf")]
        //    public async Task<IActionResult> ExportAllStudentsToPdf()
        //    {
        //        var data = await _studentService.GetAllWithDepartmentNameAsync();
        //        var fileBytes = await _reportService.ExportToPdfAsync(data, "Students Data Report");
        //        return File(fileBytes, "application/pdf", "Students Data.pdf");
        //    }  

        //    [HttpGet("ProfessorCourses/pdf")]
        //    public async Task<IActionResult> ExportProfessorCoursesToPdf()
        //    {
        //        var data = await _reportService.GetProfessorCoursesAsync();
        //        var fileBytes = await _reportService.ExportToPdfAsync(data, "Professor Courses Report");
        //        return File(fileBytes, "application/pdf", "ProfessorCourses.pdf");
        //    } 

        //    [HttpGet("students-per-department/pdf")]
        //    public async Task<IActionResult> ExportStudentsPerDepartmentToPdf()
        //    {
        //        var data = await _reportService.GetStudentsPerDepartmentAsync();
        //        var fileBytes = await _reportService.ExportToPdfAsync(data, "Students Per Department Report");
        //        return File(fileBytes, "application/pdf", "StudentsPerDepartment.pdf");
        //    }

        //    [HttpGet("AcademicWarnings/pdf")]
        //    public async Task<IActionResult> ExportAcademicWarningsToPdf()
        //    {
        //        var data = await _reportService.GetAcademicWarningsAsync();
        //        var fileBytes = await _reportService.ExportToPdfAsync(data, "Academic Warnings Report");
        //        return File(fileBytes, "application/pdf", "AcademicWarnings.pdf");
        //    } 


        [HttpGet("GetStudentsByDepartmentAndName/pdf")]
        public async Task<IActionResult> GetStudentsByDepartmentAndName(
        [FromQuery]
        string? departmentName,

        [FromQuery] string? studentName,

        [FromQuery] string? studentStatus,

        [FromQuery] string? divisionName)
        {
            var filters = new Dictionary<string, object>
            {
                ["departmentName"] = departmentName,
                ["studentName"] = studentName,
                ["studentStatus"] = studentStatus,
                ["divisionName"] = divisionName
            };
            var title = _titleGenerator.GenerateTitle("بيانات الطلاب", filters);
            var fileName = _titleGenerator.GenerateFileName("StudentData", filters);
            var data = await _studentService 
                .GetStudentsByDepartmentAndNameAsync(departmentName, studentName, studentStatus, divisionName);
            var fileBytes = await _reportService.ExportToPdfAsync(data, title);
            return File(fileBytes, "application/pdf", fileName);
        }


        [HttpGet("FilterCourses/pdf")]
        public async Task<IActionResult> GetFilteredCourses(
                                         [FromQuery] string? courseName,
                                         [FromQuery] string? departmentName,
                                         [FromQuery] string? courseStatus,
                                         [FromQuery] string? divisionName)
        {
            var filters = new Dictionary<string, object>
            {
                ["departmentName"] = departmentName,
                ["courseName"] = courseName,
                ["courseStatus"] = courseStatus,
                ["divisionName"] = divisionName
            };
            var title = _titleGenerator.GenerateTitle("بيانات الكورسات", filters);
            var fileName = _titleGenerator.GenerateFileName("CourseData", filters);
            var data = await _reportService.GetFilteredCoursesAsync(courseName, departmentName, courseStatus, divisionName);
            var fileBytes = await _reportService.ExportToPdfAsync(data, title);
            return File(fileBytes, "application/pdf", fileName);
        }

        [HttpGet("ProfessorSchedule/pdf")]
        public async Task<IActionResult> GetProfessorSchedulePdf()
        {
            try
            {
                var professorIdClaim = User.FindFirst("ProfessorId");
                if (professorIdClaim == null)
                    return Unauthorized("Invalid token: ProfessorId not found");

                int professorId = int.Parse(professorIdClaim.Value);

                // الحصول على بيانات المحاضرات
                var classes = await _classService.GetProfessorClassesAsync(professorId);

                if (!classes.Any())
                {
                    return NotFound("لا توجد محاضرات مسجلة لهذا الدكتور");
                }               
                var professorName = classes.First().ProfessorName;

                // إنشاء PDF

                var fileBytes = await _reportService.GenerateProfessorSchedulePdfAsync(classes, professorName);

                var fileName = $"جدول_المحاضرات_{professorName.Replace(" ", "_")}_{DateTime.Now:yyyyMMdd}.pdf";
                return File(fileBytes, "application/pdf", fileName);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"حدث خطأ أثناء إنشاء التقرير: {ex.Message}");
            }
        }
    }
}
