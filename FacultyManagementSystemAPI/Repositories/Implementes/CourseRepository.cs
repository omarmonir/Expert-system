using FacultyManagementSystemAPI.Data;
using FacultyManagementSystemAPI.Models.DTOs.Courses;
using FacultyManagementSystemAPI.Models.Entities;
using FacultyManagementSystemAPI.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace FacultyManagementSystemAPI.Repositories.Implementes
{
    public class CourseRepository(AppDbContext dbContext) : GenericRepository<Course>(dbContext), ICourseRepository
    {
        private readonly AppDbContext _dbContext = dbContext;

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
                    CurrentEnrolledStudents = c.Course.CurrentEnrolledStudents,
                    MaxSeats = c.Course.MaxSeats,
                    PreCourseName = c.Course.PreCourse != null ? c.Course.PreCourse.Name : "لا يوجد مقرر مطلوب لهذا المقرر",
                    //ProfessorName = c.Professor.FullName
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
                        CurrentEnrolledStudents = c.CurrentEnrolledStudents,
                        MaxSeats = c.MaxSeats,
                        Semester = c.Semester,
                        PreCourseName = c.PreCourse != null ? c.PreCourse.Name : "لا يوجد مقرر مطلوب لهذا المقرر",
                        //ProfessorName = c.Professor.FullName
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
                    CurrentEnrolledStudents = c.CurrentEnrolledStudents,
                    MaxSeats = c.MaxSeats,
                    Semester = c.Semester,
                    PreCourseName = c.PreCourse != null ? c.PreCourse.Name : "لا يوجد مقرر مطلوب لهذا المقرر",
                    //ProfessorName = c.Professor.FullName
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
                    CurrentEnrolledStudents = c.Course.CurrentEnrolledStudents,
                    MaxSeats = c.Course.MaxSeats,
                    PreCourseName = c.Course.PreCourse != null ? c.Course.Name : "لا يوجد مقرر مطلوب لهذا المقرر",
                })
                .ToListAsync();
        }

        public async Task<IEnumerable<CourseDto>> GetCoursesByProfessorIdWithPreCourseNameAsync(int professorId)
        {
            return await _dbContext.Classes
               .Where(c => c.ProfessorId == professorId)
               .Select(c => new CourseDto
               {
                   Id = c.Id,
                   Name = c.Course.Name,
                   Description = c.Course.Description,
                   Credits = c.Course.Credits,
                   Status = c.Course.Status,
                   Code = c.Course.Code,
                   Semester = c.Course.Semester,
                   CurrentEnrolledStudents = c.Course.CurrentEnrolledStudents,
                   MaxSeats = c.Course.MaxSeats,
                   PreCourseName = c.Course.PreCourse != null ? c.Course.PreCourse.Name : "لا يوجد مقرر مطلوب لهذا المقرر",
                   //ProfessorName = c.Professor.FullName
               })
               .Distinct()
               .ToListAsync();
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
                CurrentEnrolledStudents = c.CurrentEnrolledStudents,
                MaxSeats = c.MaxSeats,
                Semester = c.Semester,
                PreCourseName = c.PreCourse != null ? c.PreCourse.Name : "لا يوجد مقرر مطلوب لهذا المقرر",
                //ProfessorName = c.Professor.FullName
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
                    //ProfessorName = e.Course.Classes.Any() ? e.Course.Classes.FirstOrDefault().Professor.FullName : "غير محدد",
                    //PreCourseName = e.Course.PreCourse.Name

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
            CurrentEnrolledStudents = c.Course.CurrentEnrolledStudents,
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

    }
}
