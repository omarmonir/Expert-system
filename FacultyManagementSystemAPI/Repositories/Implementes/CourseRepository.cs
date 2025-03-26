using FacultyManagementSystemAPI.Data;
using FacultyManagementSystemAPI.Models.DTOs.Courses;
using FacultyManagementSystemAPI.Models.Entities;
using FacultyManagementSystemAPI.Repositories.Interfaces;
using Gherkin.CucumberMessages.Types;
using Microsoft.EntityFrameworkCore;
using System.Linq;

namespace FacultyManagementSystemAPI.Repositories.Implementes
{
    public class CourseRepository(AppDbContext dbContext) : GenericRepository<Course>(dbContext), ICourseRepository
    {
        private readonly AppDbContext _dbContext = dbContext;
        private const decimal PASSING_GRADE = 60;
        public async Task<IEnumerable<CourseDto>> SearchCoursesWithPreCourseNameAsync(string searchTerm)
        {
           

            return await _dbContext.Classes
                .AsNoTrackingWithIdentityResolution()
                .Where(c => c.Course.Name.Contains(searchTerm))
                .Select(c => new CourseDto
                {
                    Id = c.CourseId,
                    Name = c.Course.Name,
                    Description = c.Course.Description,
                    Credits = c.Course.Credits,
                    Status = c.Course.Status,
                    Semester = c.Course.Semester,
                    Code = c.Course.Code,
                    CurrentEnrolledStudents = _dbContext.Enrollments.Count(e => e.CourseId == c.Course.Id),
                    MaxSeats = c.Course.MaxSeats,
                    PreCourseName = c.Course.PreCourse != null ? c.Course.PreCourse.Name : "لا يوجد مقرر مطلوب لهذا المقرر",
                    ProfessorName = c.Professor.FullName,
                    DepartmentName = c.Course.CourseDepartments.Select(d => d.Department.Name).FirstOrDefault()??  "لا يوجد قسم لهذا المقرر"
                })
                .ToListAsync();
        }
        public async Task<IEnumerable<CourseDto>> GetAllWithPreCourseNameAsync()
        {

            return await _dbContext.Courses
                    .AsNoTrackingWithIdentityResolution()
                    .Select(c => new CourseDto
                    {
                        Id = c.Id,
                        Name = c.Name,
                        Description = c.Description,
                        Credits = c.Credits,
                        Status = c.Status,
                        Code = c.Code,
                        CurrentEnrolledStudents = _dbContext.Enrollments.Count(e => e.CourseId == c.Id),
                        MaxSeats = c.MaxSeats,
                        Semester = c.Semester,
                        PreCourseName = c.PreCourse != null ? c.PreCourse.Name : "لا يوجد مقرر مطلوب لهذا المقرر",
                        ProfessorName = c.Classes.Select( c => c.Professor.FullName).FirstOrDefault() ?? "لا يوجد دكتور لهذا المقرر",
                        DepartmentName = c.CourseDepartments.Select(d => d.Department.Name).FirstOrDefault() ?? "لا يوجد قسم لهذا المقرر"
                    })
                                 .ToListAsync();
        }

        public async Task<IEnumerable<CourseDto>> GetCoursesBySemesterWithPreCourseNameAsync(byte semester)
        {
            return await _dbContext.Courses
                .AsNoTrackingWithIdentityResolution()
                .Where(c => c.Semester == semester)
                .Select(c => new CourseDto
                {
                    Id = c.Id,
                    Name = c.Name,
                    Description = c.Description,
                    Credits = c.Credits,
                    Status = c.Status,
                    Code = c.Code,
                    CurrentEnrolledStudents = _dbContext.Enrollments.Count(e => e.CourseId == c.Id),
                    MaxSeats = c.MaxSeats,
                    Semester = c.Semester,
                    PreCourseName = c.PreCourse != null ? c.PreCourse.Name : "لا يوجد مقرر مطلوب لهذا المقرر",
                    ProfessorName = c.Classes.Select(c => c.Professor.FullName).FirstOrDefault() ?? "لا يوجد دكتور لهذا المقرر",
                    DepartmentName = c.CourseDepartments.Select(d => d.Department.Name).FirstOrDefault() ?? "لا يوجد قسم لهذا المقرر"
                })
                .ToListAsync();
        }

        public async Task<IEnumerable<CourseDto>> GetCoursesByDepartmentIdWithPreCourseNameAsync(int departmentId)
        {
            return await _dbContext.CourseDepartments
                .AsNoTrackingWithIdentityResolution()
                .Where(c => c.DepartmentId == departmentId)
                .Select(c => new CourseDto
                {
                    Id = c.Id,
                    Name = c.Course.Name,
                    Description = c.Course.Description,
                    Credits = c.Course.Credits,
                    Status = c.Course.Status,
                    Code = c.Course.Code,
                    Semester = c.Course.Semester,
                    CurrentEnrolledStudents = _dbContext.Enrollments.Count(e => e.CourseId == c.Course.Id),
                    MaxSeats = c.Course.MaxSeats,
                    PreCourseName = c.Course.PreCourse != null ? c.Course.Name : "لا يوجد مقرر مطلوب لهذا المقرر",
                    DepartmentName = c.Department.Name,
                    ProfessorName = c.Course.Classes.Select(c => c.Professor.FullName).FirstOrDefault() ?? "لا يوجد دكتور لهذا المقرر"
                })
                .ToListAsync();
        }

        public async Task<IEnumerable<CourseDto>> GetCoursesByProfessorIdWithPreCourseNameAsync(int professorId)
        {
            var courses = await _dbContext.Classes
                .Where(c => c.ProfessorId == professorId)
                .Select(c => new CourseDto
                {
                    Id = c.CourseId,
                    Name = c.Course.Name,
                    Description = c.Course.Description,
                    Credits = c.Course.Credits,
                    Status = c.Course.Status,
                    Semester = c.Course.Semester,
                    Code = c.Course.Code,
                    CurrentEnrolledStudents = _dbContext.Enrollments.Count(e => e.CourseId == c.Course.Id),
                    MaxSeats = c.Course.MaxSeats,
                    PreCourseName = c.Course.PreCourse != null ? c.Course.PreCourse.Name : "لا يوجد مقرر مطلوب لهذا المقرر",
                    ProfessorName = c.Professor.FullName,
                    DepartmentName = c.Course.CourseDepartments.Select(d => d.Department.Name).FirstOrDefault() ?? "لا يوجد قسم لهذا المقرر",

                    // حساب معدل الحضور للمقرر
                    AttendanceRate = _dbContext.Attendances
                        .Where(a => a.Class.CourseId == c.CourseId)
                        .Any()
                        ? Math.Round(
                            (double)_dbContext.Attendances
                                .Where(a => a.Class.CourseId == c.CourseId)
                                .Count(a => a.Status) /
                            _dbContext.Attendances
                                .Where(a => a.Class.CourseId == c.CourseId)
                                .Count() * 100, 2)
                        : (double?)null,
                })
                .Distinct()
                .ToListAsync();

            // هنا نقوم بحساب المعدل في الذاكرة بعد تحميل البيانات
            foreach (var course in courses)
            {
                var finalGrades = _dbContext.Enrollments
                    .Where(e => e.CourseId == course.Id )
                    .Select(e => (e.Exam1Grade.GetValueOrDefault() + e.Exam2Grade.GetValueOrDefault() + e.Grade.GetValueOrDefault()))
                    .ToList();

                // حساب المعدل النهائي
                course.AverageFinalGrade = finalGrades.Any() ? Math.Round(finalGrades.Average(), 2) : (decimal?)null;
            }

            return courses;
        }


        public async Task<bool> CourseExistsAsync(string courseName)
        {
            return await _dbContext.Courses
                 .AnyAsync(d => d.Name == courseName);
        }

        public async Task<CourseDto> GetByIdWithPreCourseNameAsync(int id)
        {
            var courseDto = await _dbContext.Courses
            .AsNoTrackingWithIdentityResolution()
            .Where(c => c.Id == id)
            .Select(c => new CourseDto
            {
                Id = c.Id,
                Name = c.Name,
                Description = c.Description,
                Credits = c.Credits,
                Status = c.Status,
                Code = c.Code,
                CurrentEnrolledStudents = _dbContext.Enrollments.Count(e => e.CourseId == c.Id),
                MaxSeats = c.MaxSeats,
                Semester = c.Semester,
                PreCourseName = c.PreCourse != null ? c.PreCourse.Name : "لا يوجد مقرر مطلوب لهذا المقرر",
                ProfessorName = c.Classes.Select(c => c.Professor.FullName).FirstOrDefault() ?? "لا يوجد دكتور لهذا المقرر",
                DepartmentName = c.CourseDepartments.Select(d => d.Department.Name).FirstOrDefault() ?? "لا يوجد قسم لهذا المقرر"
            })

                .FirstOrDefaultAsync();
            return courseDto;
        }

        public async Task<bool> CourseExistsAsync(int? PreCourseId)
        {
            return await _dbContext.Courses
                 .AnyAsync(d => d.Id == PreCourseId);
        }

        public async Task<int> CountAsync()
        {
            return await _dbContext.Courses.CountAsync();
        }

        public async Task<int> CountByStatusAsync()
        {
            return await _dbContext.Courses
                .Where(c => c.Status == "نشط")
                .CountAsync();
        }

        public async Task<IEnumerable<string>> GetAllPreRequisiteCoursesAsync()
        {
            return await _dbContext.Courses
                .Where(c => c.PreCourseId != null) // Only get courses with prerequisites
                .Select(c => c.PreCourse.Name) // Select the name of the prerequisite course
                .Distinct() // Remove duplicates
                .ToListAsync();
        }

        public async Task<IEnumerable<CourseRegistrationStatsDto>> GetCourseRegistrationStatsByCourseOverTimeAsync(int courseId)
        {
            return await _dbContext.Enrollments
            .Where(e => e.CourseId == courseId)
            .GroupBy(e => new { e.AddedEnrollmentDate.Year, e.AddedEnrollmentDate.Month })
            .Select(g => new CourseRegistrationStatsDto
            {
                CourseId = courseId,
                CourseName = g.First().Course.Name,
                Year = g.Key.Year,
                Month = g.Key.Month,
                TotalRegistrations = g.Count(e => e.DeletedEnrollmentDate == null), // التسجيل الساري
                TotalCancellations = g.Count(e => e.DeletedEnrollmentDate != null)  // التسجيل الملغى
            })
            .ToListAsync();
        }

        public async Task<List<CourseDto>> GetCoursesByStudentIdAsync(int studentId)
        {
            return await _dbContext.Enrollments
                .Where(e => e.StudentId == studentId)
                .Include(e => e.Course)
                 .ThenInclude(c => c.Classes) // تضمين جدول Class
                  .ThenInclude(cl => cl.Professor) // تضمين الدكتور المسؤول عن الكلاس
                .Select(e => new CourseDto
                {
                    Id = e.Course.Id,
                    Name = e.Course.Name,
                    Description = e.Course.Description,
                    Credits = e.Course.Credits,
                    Status = e.Course.Status,
                    Code = e.Course.Code,
                    Semester = e.Course.Semester,
                    MaxSeats = e.Course.MaxSeats,
                    ProfessorName = e.Course.Classes.Select(cls => cls.Professor.FullName)
                                                                        .FirstOrDefault() ?? "لا يوجد مدرس لهذا المقرر",
                    PreCourseName = e.Course.PreCourse.Name != null ? e.Course.PreCourse.Name : "لا يوجد مقرر مطلوب لهذا المقرر",
                    DepartmentName = e.Course.CourseDepartments.Select(cd => cd.Department.Name)
                                                                         .FirstOrDefault() ?? "لا يوجد قسم لهذاالمقرر"
                })
                .Distinct()
                .ToListAsync();
        }

        public async Task<IEnumerable<string>> GetAllCoursesStatusesAsync()
        {
            var status = await _dbContext.Courses
                .Select(d => d.Status)
                .Distinct()
                .ToListAsync();

            return status;
        }

        public async Task<IEnumerable<string>> GetAllCoursesNameAsync()
        {
            var names = await _dbContext.Courses
           .Select(d => d.Name)
           .ToListAsync();

            return names;
        }

        public async Task<IEnumerable<FilterCourseDto>> GetFilteredCoursesAsync(string? courseName, string? departmentName, string? courseStatus)
        {
            var courseQuery = _dbContext.CourseDepartments
        .AsNoTracking()
        .Select(c => new FilterCourseDto
        {
            Id = c.Course.Id,
            Name = c.Course.Name,
            Description = c.Course.Description,
            Credits = c.Course.Credits,
            Status = c.Course.Status,
            Code = c.Course.Code,
            Semester = c.Course.Semester,
            MaxSeats = c.Course.MaxSeats,
            CurrentEnrolledStudents = _dbContext.Enrollments.Count(e => e.CourseId == c.Course.Id),
            PreCourseName = c.Course.PreCourse.Name != null ? c.Course.PreCourse.Name : "لا يوجد مقرر مطلوب لهذا المقرر",
            DepartmentName = c.Department != null ? c.Department.Name : "لا يوجد قسم لهذا المقرر"
        });

            // تطبيق الفلاتر على مستوى قاعدة البيانات
            if (!string.IsNullOrWhiteSpace(courseStatus))
            {
                courseQuery = courseQuery.Where(c => c.Status.Contains(courseStatus));
            }

            // جلب البيانات إلى الذاكرة قبل تطبيق الفلاتر النصية
            var courses = await courseQuery.ToListAsync();

            // تنفيذ الفلترة النصية على مستوى الذاكرة
            if (!string.IsNullOrWhiteSpace(courseName))
            {
                courseName = NormalizeArabicText(courseName);
                courses = courses
                    .Where(c => NormalizeArabicText(c.Name).Contains(courseName))
                    .ToList();
            }


            if (!string.IsNullOrWhiteSpace(departmentName))
            {
                departmentName = NormalizeArabicText(departmentName);
                courses = courses
                    .Where(c => !string.IsNullOrEmpty(c.DepartmentName) && NormalizeArabicText(c.DepartmentName).Contains(departmentName))
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

        public async Task<CourseStatisticsDto> GetCourseStatisticsAsync(int courseId)
        {
            var enrollments = await _dbContext.Enrollments
            .AsNoTracking()
            .Where(e => e.CourseId == courseId)
            .ToListAsync();

            int enrolledStudentsCount = enrollments.Count;

            int departmentsCount = await _dbContext.CourseDepartments
           .AsNoTracking()
           .Where(cd => cd.CourseId == courseId)
           .CountAsync();

            decimal averageGrade = 0;
            if (enrollments.Any())
            {
                averageGrade = enrollments
                    .Where(e => e.FinalGrade.HasValue)
                    .Select(e => e.FinalGrade.Value)
                    .DefaultIfEmpty(0)
                    .Average();
            }

            decimal successRate = 0;
            int totalStudentsWithGrades = enrollments.Count(e => e.FinalGrade.HasValue);
            if (totalStudentsWithGrades > 0)
            {
                int passedStudents = enrollments.Count(e =>
                    e.FinalGrade.HasValue && e.FinalGrade.Value >= PASSING_GRADE);
                successRate = (decimal)passedStudents / totalStudentsWithGrades * 100;
            }
            var statistics = new CourseStatisticsDto
            {
                CourseId = courseId,
                EnrolledStudentsCount = enrolledStudentsCount,
                DepartmentsCount = departmentsCount,
                AverageGrade = Math.Round(averageGrade, 2),
                SuccessRate = Math.Round(successRate, 2)
            };

            return statistics;
        }

        public async Task<int> CountActiveCourseAsync()
        {
            return await _dbContext.Courses
                 .Where(e => e.Status == "نشط")
                 .CountAsync();
        }
        public async Task<IEnumerable<CourseDto>> SearchCoursesWithCourseNameAndStatusAsync(string searchTerm, string status)
        {
            return await _dbContext.Classes
                 .AsNoTrackingWithIdentityResolution()
                 .Where(c => c.Course.Name.Contains(searchTerm) && c.Course.Status == status)
                 .Select(c => new CourseDto
                 {
                     Id = c.CourseId,
                     Name = c.Course.Name,
                     Description = c.Course.Description,
                     Credits = c.Course.Credits,
                     Status = c.Course.Status,
                     Semester = c.Course.Semester,
                     Code = c.Course.Code,
                     CurrentEnrolledStudents = _dbContext.Enrollments.Count(e => e.CourseId == c.Course.Id),
                     MaxSeats = c.Course.MaxSeats,
                     PreCourseName = c.Course.PreCourse != null ? c.Course.PreCourse.Name : "لا يوجد مقرر مطلوب لهذا المقرر",
                     ProfessorName = c.Professor.FullName,
                     DepartmentName = c.Course.CourseDepartments.Select(d => d.Department.Name).FirstOrDefault() ?? "لا يوجد قسم لهذا المقرر"
                 })
                 .ToListAsync();
        }

    }
}
