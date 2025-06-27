using FacultyManagementSystemAPI.Models.DTOs.professors;
using FacultyManagementSystemAPI.Services.Implementes;
using FacultyManagementSystemAPI.Services.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using static PdfService;

namespace FacultyManagementSystemAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ReportsController(IReportService reportService, IStudentService studentService, IEnrollmentService enrollmentService,
        ICourseService courseService, IClassService classService, IProfessorService professorService) : ControllerBase
    {
        private readonly IReportService _reportService = reportService;
        private readonly IStudentService _studentService = studentService;
        private readonly ICourseService _courseService = courseService;
        private readonly IClassService _classService = classService;
        private readonly IEnrollmentService _enrollmentService = enrollmentService;
        private readonly IProfessorService _professorService = professorService;
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

        [HttpGet("StudentsWithExamGradesByCourseId/{courseId}")]
        public async Task<IActionResult> GetStudentsWithExamGradesByCourseId(int courseId)
        {
            try
            {
                // إنشاء الفيلتر للعنوان واسم الملف
                var filters = await _courseService.GetByIdWithPreCourseNameAsync(courseId);


                // جلب البيانات
                var students = await _studentService.GetStudentsWithExamGradesByCourseIdAsync(courseId);

                // تصدير البيانات إلى PDF
                var fileBytes = await _reportService.ExportToPdfAsync(students, $"درجات الطلاب في كورس {filters.Name}");

                // إرجاع الملف
                return File(fileBytes, "application/pdf", "StudentGrades");
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
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
        [HttpGet("StudentEnrollment/pdf")]
        public async Task<IActionResult> GetEnrollment([FromQuery] string? studentName)
        {
            var filters = new Dictionary<string, object>
            {
                ["studentName"] = studentName,
               
            };
            var title = _titleGenerator.GenerateTitle("بيانات التسجيلات", filters);
            var fileName = _titleGenerator.GenerateFileName("Enrollment", filters);
            var data = await _enrollmentService.GetEnrollmentsAsync(studentName);
            var fileBytes = await _reportService.ExportToPdfAsync(data, title);
            return File(fileBytes, "application/pdf", fileName);
        } 
        
        [HttpGet("FilterProfessor/pdf")]
        public async Task<ActionResult<IEnumerable<ProfessorDto>>> GetProfessorsByFilters(
        [FromQuery] string? departmentName,
        [FromQuery] string? professorName,
        [FromQuery] string? Position)
        {
            var filters = new Dictionary<string, object>
            {
                ["departmentName"] = departmentName,
                ["professorName"] = professorName,               
            };
            var title = _titleGenerator.GenerateTitle("بيانات الدكاترة", filters);
            var fileName = _titleGenerator.GenerateFileName("ProfessorData", filters);
            var data = await _professorService.GetProfessorsByFiltersAsync(departmentName, professorName, Position);
            var fileBytes = await _reportService.ExportToPdfAsync(data, title);
            return File(fileBytes, "application/pdf", fileName);
        }
        
        [HttpGet("classes/pdf")]
        public async Task<IActionResult> GetAllClassesWithProfNameAndCourseName(
        [FromQuery] string? level,
        [FromQuery] string? divisionName)
        {
            var filters = new Dictionary<string, object>
            {
                ["level"] = level,
                ["divisionName"] = divisionName
            };
            var title = _titleGenerator.GenerateTitle("بيانات المحاضرات", filters);
            var fileName = _titleGenerator.GenerateFileName("المحاضرات", filters);
            var data = await _classService
                    .GetAllClassesWithProfNameAndCourseNameAsyncOptimized(divisionName, level);
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
        [HttpGet("StudentSchedule/pdf")]
        public async Task<IActionResult> GetClasses()
        {
            try
            {
                var studentIdClaim = User.FindFirst("StudentId");
            if (studentIdClaim == null)
                return Unauthorized("Invalid token: StudentId not found");

            int studentId = int.Parse(studentIdClaim.Value);
            var classes = await _classService.GetStudentClassesAsync(studentId);

            if (!classes.Any())
            {
                return NotFound("لا توجد محاضرات مسجلة لهذا الطالب");
            }

            // Await the async method call
            var studentName = await _studentService.GetStudentNameById(studentId);

            var fileBytes = await _reportService.GenerateStudentSchedulePdfAsync(classes, studentName);
            var fileName = $"جدول_المحاضرات_{studentName.Replace(" ", "_")}_{DateTime.Now:yyyyMMdd}.pdf";

            return File(fileBytes, "application/pdf", fileName);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"حدث خطأ أثناء إنشاء التقرير: {ex.Message}");
            }
        }
        [HttpGet("pdf")]
        public async Task<IActionResult> GetClassesPdf(
            [FromQuery] string? divisionName = null,
            [FromQuery] string? level = null)
        {
            try
            {
               

                // جلب البيانات مع الفلترة
                var classes = await _classService.GetAllClassesWithProfNameAndCourseNameAsyncOptimized(
                    divisionName, level);

                if (!classes.Any())
                {
                    return NotFound(new { Message = "لا توجد محاضرات لإنشاء التقرير" });
                }

                // إنشاء معلومات الفلترة للتقرير
                var filterInfo = BuildFilterInfo(divisionName, level);

                // إنشاء ملف PDF
                var pdfBytes = await _reportService.GenerateAdminClassesPdfAsync(classes, filterInfo);

                // إنشاء اسم الملف
                var fileName = BuildPdfFileName(divisionName, level);

                return File(pdfBytes, "application/pdf", fileName);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    Success = false,
                    Message = "حدث خطأ أثناء إنشاء التقرير",
                    Error = ex.Message
                });
            }
        }
        private string BuildFilterInfo(string? divisionName, string? level)
        {
            var filterParts = new List<string>();

            if (!string.IsNullOrEmpty(divisionName))
                filterParts.Add($"الشعبة: {divisionName}");

            if (!string.IsNullOrEmpty(level))
                filterParts.Add($"الفرقة: {level}");

            return filterParts.Any() ? string.Join(" | ", filterParts) : null;
        }
        private string BuildPdfFileName(string? divisionName, string? level)
        {
            var fileName = "جدول_المحاضرات";

            if (!string.IsNullOrEmpty(divisionName))
                fileName += $"_شعبة_{divisionName.Replace(" ", "_")}";

            if (!string.IsNullOrEmpty(level))
                fileName += $"_فرقة_{level.Replace(" ", "_")}";

            fileName += $"_{DateTime.Now:yyyy-MM-dd}.pdf";

            return fileName;
        }
    }
}
