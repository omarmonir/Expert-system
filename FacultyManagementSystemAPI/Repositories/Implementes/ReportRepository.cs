using FacultyManagementSystemAPI.Data;
using FacultyManagementSystemAPI.Models.DTOs.Courses;
using FacultyManagementSystemAPI.Models.DTOs.Report;
using FacultyManagementSystemAPI.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace FacultyManagementSystemAPI.Repositories.Implementes
{
    public class ReportRepository(AppDbContext dbContext) : IReportRepository
    {
        private readonly AppDbContext _dbContext = dbContext;

        public async Task<IEnumerable<AcademicWarningDto>> GetAcademicWarningsAsync()
        {
            // تحميل جميع الطلاب من قاعدة البيانات
            var students = await _dbContext.Students
                .AsNoTracking()
                .ToListAsync(); // تحميل البيانات إلى الذاكرة

            // تصفية الطلاب الذين لديهم GPA أقل من 2
            var warnings = students
                .Where(s =>
                    new List<decimal?> { s.GPA1, s.GPA2, s.GPA3, s.GPA4, s.GPA5, s.GPA6, s.GPA7, s.GPA8 }
                        .Where(gpa => gpa.HasValue) // إزالة القيم null
                        .Select(gpa => gpa.Value)   // تحويل إلى decimal
                        .DefaultIfEmpty(0)          // تجنب الأخطاء عند عدم وجود بيانات
                        .Average() < 2.0m)
                .Select(s => new AcademicWarningDto
                {
                    StudentName = s.Name,
                    GPA = new List<decimal?> { s.GPA1, s.GPA2, s.GPA3, s.GPA4, s.GPA5, s.GPA6, s.GPA7, s.GPA8 }
                            .Where(gpa => gpa.HasValue)
                            .Select(gpa => gpa.Value)
                            .DefaultIfEmpty(0)
                            .Average(),
                    Email = s.Email
                })
                .ToList();

            return warnings;
        }


        public Task<IEnumerable<CourseEnrollmentDto>> GetCourseEnrollmentTrendsAsync()
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<CourseGradeReportDto>> GetCourseGradesAsync()
        {
            throw new NotImplementedException();
        }
        public async Task<IEnumerable<ProfessorCoursesDto>> GetProfessorCoursesAsync()
        {
            return await _dbContext.Professors
                .Include(p => p.Classes)
                .ThenInclude(c => c.Course)
                .Select(p => new ProfessorCoursesDto
                {
                    ProfessorName = p.FullName,
                    CourseNames = p.Classes.Any()
                        ? string.Join(", ", p.Classes.Select(c => c.Course.Name))
                        : "لا يوجد مقررات لهذا الدكتور" // ✅ إذا لم يكن هناك مقررات
                }).ToListAsync();
        }

        public Task<IEnumerable<StudentAttendanceDto>> GetStudentAttendanceAsync()
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<StudentPerCourseDto>> GetStudentsPerCourseAsync()
        {
            throw new NotImplementedException();
        }

        //public async Task<IEnumerable<StudentPerDepartmentDto>> GetStudentsPerDepartmentAsync()
        //{
        //    return await _dbContext.Students
        //    .GroupBy(s => s.Department.Name)
        //    .Select(g => new StudentPerDepartmentDto
        //    {
        //        DepartmentName = g.Key,
        //        StudentsCount = g.Count()
        //    }).ToListAsync();
        //}
        public async Task<IEnumerable<FilterDto>> GetFilteredCoursesAsync(string? courseName, string? departmentName, string? courseStatus, string? divisionName)
        {
            var query = _dbContext.Courses
                .Include(c => c.Prerequisites)
                .Include(c => c.CourseDivisions)
                    .ThenInclude(cd => cd.Division)
                        .ThenInclude(d => d.Department)
                .AsNoTracking()
                .Select(c => new FilterDto
                {
                    Id = c.Id,
                    Name = c.Name,
                    Description = c.Description,
                    Credits = c.Credits,
                    Status = c.Status,
                    Code = c.Code,
                    Semester = c.Semester,
                    MaxSeats = c.MaxSeats,
                    CurrentEnrolledStudents = _dbContext.Enrollments.Count(e => e.CourseId == c.Id),
                    PreCourseName = c.Prerequisites.Any()
                        ? string.Join("، ", c.Prerequisites.Select(p => p.PrerequisiteCourse.Name))
                        : "لا يوجد مقرر مطلوب لهذا المقرر",
                    DepartmentName = c.CourseDivisions
                                      .Select(cd => cd.Division.Department.Name)
                                      .Distinct()
                                      .FirstOrDefault() ?? "لا يوجد قسم لهذا المقرر",
                    DivisionNames = c.CourseDivisions.Any(cd => cd.Division != null)
                    ? string.Join("، ", c.CourseDivisions
                        .Where(cd => cd.Division != null)
                        .Select(cd => cd.Division.Name)
                        .Distinct())
                    : "لا توجد شعب لهذا المقرر"
                });

            // فلترة حسب الحالة
            if (!string.IsNullOrWhiteSpace(courseStatus))
            {
                query = query.Where(c => c.Status.Contains(courseStatus));
            }

            var courses = await query.ToListAsync();

            // فلترة حسب اسم الكورس
            if (!string.IsNullOrWhiteSpace(courseName))
            {
                courseName = NormalizeArabicText(courseName);
                courses = courses
                    .Where(c => NormalizeArabicText(c.Name).Contains(courseName))
                    .ToList();
            }

            // فلترة حسب القسم
            if (!string.IsNullOrWhiteSpace(departmentName))
            {
                departmentName = NormalizeArabicText(departmentName);
                courses = courses
                    .Where(c => !string.IsNullOrEmpty(c.DepartmentName)
                                && NormalizeArabicText(c.DepartmentName).Contains(departmentName))
                    .ToList();
            }
            if (!string.IsNullOrWhiteSpace(divisionName))
            {
                divisionName = NormalizeArabicText(divisionName);
                courses = courses
                    .Where(c => !string.IsNullOrWhiteSpace(c.DivisionNames) &&
                                NormalizeArabicText(c.DivisionNames).Contains(divisionName))
                    .ToList();
            }



            return courses;
        }



        private string NormalizeArabicText(string text)
        {
            return text.Replace('أ', 'ا')
                       .Replace('إ', 'ا')
                       .Replace('آ', 'ا')
                       .Replace('ى', 'ي')
                       .Replace('ه', 'ة');
        }
    }
}
