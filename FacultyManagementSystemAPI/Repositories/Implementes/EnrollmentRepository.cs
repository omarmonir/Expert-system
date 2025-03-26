using FacultyManagementSystemAPI.Data;
using FacultyManagementSystemAPI.Models.DTOs.Enrollment;
using FacultyManagementSystemAPI.Models.Entities;
using FacultyManagementSystemAPI.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace FacultyManagementSystemAPI.Repositories.Implementes
{
    public class EnrollmentRepository(AppDbContext dbContext) : GenericRepository<Enrollment>(dbContext), IEnrollmentRepository
    {
        private readonly AppDbContext _dbContext = dbContext;

        public async Task<IEnumerable<EnrollmentDto>> GetAllIncludeStudentNameCourseNameAsync()
        {
            var enrollments = await _dbContext.Enrollments
                .Select(e => new EnrollmentDto
                {
                    Id = e.Id,
                    StudentID = e.StudentId,
                    StudentName = _dbContext.Students
                        .Where(s => s.Id == e.StudentId)
                        .Select(s => s.Name)
                        .FirstOrDefault() ?? "غير معروف",

                    CourseCode = _dbContext.Courses
                        .Where(c => c.Id == e.CourseId)
                        .Select(c => c.Code)
                        .FirstOrDefault() ?? "غير معروف",

                    CourseName = _dbContext.Courses
                        .Where(c => c.Id == e.CourseId)
                        .Select(c => c.Name)
                        .FirstOrDefault() ?? "غير معروف",

                    EnrollmentDate = e.AddedEnrollmentDate,
                    EnrollmentStatus = e.IsCompleted,
                    Semester = e.Semester
                })
                .ToListAsync();

            return enrollments;
        }

        public async Task<EnrollmentDto> GetByIdIncludeStudentNameCourseNameAsync(int id)
        {
            var enrollmentDto = await _dbContext.Enrollments
                 .AsNoTrackingWithIdentityResolution()
                .Where(e => e.Id == id)
                .Select(e => new EnrollmentDto
                {
                    StudentID = e.StudentId,
                    StudentName = _dbContext.Students
                        .Where(s => s.Id == e.StudentId)
                        .Select(s => s.Name)
                        .FirstOrDefault() ?? "غير معروف",

                    CourseCode = _dbContext.Courses
                        .Where(c => c.Id == e.CourseId)
                        .Select(c => c.Code)
                        .FirstOrDefault() ?? "غير معروف",

                    CourseName = _dbContext.Courses
                        .Where(c => c.Id == e.CourseId)
                        .Select(c => c.Name)
                        .FirstOrDefault() ?? "غير معروف",

                    EnrollmentDate = e.AddedEnrollmentDate,
                    EnrollmentStatus = e.IsCompleted,
                    Semester = e.Semester
                })
                .FirstOrDefaultAsync();

            return enrollmentDto;
        }

        public async Task<IEnumerable<EnrollmentDto>> GetBySemesterAsync(string name)
        {
            var enrollments = await _dbContext.Enrollments
                .Where(e => e.Semester.Contains(name))
                .AsNoTrackingWithIdentityResolution()
                .Select(e => new EnrollmentDto
                {
                    StudentID = e.StudentId,
                    StudentName = _dbContext.Students
                        .Where(s => s.Id == e.StudentId)
                        .Select(s => s.Name)
                        .FirstOrDefault() ?? "غير معروف",

                    CourseCode = _dbContext.Courses
                        .Where(c => c.Id == e.CourseId)
                        .Select(c => c.Code)
                        .FirstOrDefault() ?? "غير معروف",

                    CourseName = _dbContext.Courses
                        .Where(c => c.Id == e.CourseId)
                        .Select(c => c.Name)
                        .FirstOrDefault() ?? "غير معروف",

                    EnrollmentDate = e.AddedEnrollmentDate,
                    EnrollmentStatus = e.IsCompleted,
                    Semester = e.Semester
                })
                .ToListAsync();

            return enrollments;
        }

        public async Task<IEnumerable<EnrollmentDto>> GetByStudentIdAsync(int studentId)
        {
            return await _dbContext.Enrollments
                .AsNoTrackingWithIdentityResolution()
                .Where(e => e.StudentId == studentId)
                .Select(e => new EnrollmentDto
                {
                    StudentID = e.StudentId,
                    StudentName = _dbContext.Students
                        .Where(s => s.Id == e.StudentId)
                        .Select(s => s.Name)
                        .FirstOrDefault() ?? "غير معروف",

                    CourseCode = _dbContext.Courses
                        .Where(c => c.Id == e.CourseId)
                        .Select(c => c.Code)
                        .FirstOrDefault() ?? "غير معروف",

                    CourseName = _dbContext.Courses
                        .Where(c => c.Id == e.CourseId)
                        .Select(c => c.Name)
                        .FirstOrDefault() ?? "غير معروف",

                    EnrollmentDate = e.AddedEnrollmentDate,
                    EnrollmentStatus = e.IsCompleted,
                    Semester = e.Semester
                })
                .ToListAsync();
        }

        public async Task<IEnumerable<EnrollmentDto>> GetByCourseIdAsync(int courseId)
        {
            return await _dbContext.Enrollments
                .AsNoTrackingWithIdentityResolution()
                .Where(e => e.CourseId == courseId)
                .Select(e => new EnrollmentDto
                {
                    StudentID = e.StudentId,
                    StudentName = _dbContext.Students
                        .Where(s => s.Id == e.StudentId)
                        .Select(s => s.Name)
                        .FirstOrDefault() ?? "غير معروف",

                    CourseCode = _dbContext.Courses
                        .Where(c => c.Id == e.CourseId)
                        .Select(c => c.Code)
                        .FirstOrDefault() ?? "غير معروف",

                    CourseName = _dbContext.Courses
                        .Where(c => c.Id == e.CourseId)
                        .Select(c => c.Name)
                        .FirstOrDefault() ?? "غير معروف",

                    EnrollmentDate = e.AddedEnrollmentDate,
                    EnrollmentStatus = e.IsCompleted,
                    Semester = e.Semester
                })
                .ToListAsync();
        }

        public async Task<int> CountAsync()
        {
            return await _dbContext.Enrollments.CountAsync();
        }

        public async Task<int> GetCompletedEnrollmentCountAsync()
        {
            return await _dbContext.Enrollments.CountAsync(e => e.IsCompleted == "ناجح");
        }


        public async Task<int> GetAllWaitEnrollmentStudentsCountAsync()
        {
            return await _dbContext.Enrollments.CountAsync(e => e.IsCompleted == "قيد الدراسة");
        }

        public async Task<int> CountDeletedAsync()
        {
            return await _dbContext.Enrollments.CountAsync(e => e.DeletedEnrollmentDate != null);
        }

        public async Task<int> GetAllEnrollmentStudentsCountAsync()
        {
            return await _dbContext.Enrollments
                .Select(e => e.StudentId)
                .Distinct()
                .CountAsync();
        }
        public async Task<Enrollment> GetByStudentAndCourseIdAsync(int studentId, int courseId)
        {
            return await _dbContext.Enrollments
                .FirstOrDefaultAsync(e => e.StudentId == studentId && e.CourseId == courseId);
        }

        public async Task UpdateAsync(Enrollment enrollment)
        {
            _dbContext.Enrollments.Update(enrollment);
            await _dbContext.SaveChangesAsync();
        }

        public async Task<IEnumerable<EnrollmentDto>> GetFilteredEnrollmentsAsync(string? studentName, string? courseName, string? enrollmentStatus, string? semester)
        {
            var enrollmentQuery = _dbContext.Enrollments
                .AsNoTracking()
                .Select(e => new EnrollmentDto
                {
                    StudentID = e.StudentId,
                    StudentName = e.Student.Name,
                    CourseCode = e.Course.Code,
                    CourseName = e.Course.Name,
                    EnrollmentDate = e.AddedEnrollmentDate,
                    EnrollmentStatus = e.IsCompleted,
                    Semester = e.Semester
                });

            // تنفيذ الفلترة على مستوى قاعدة البيانات فقط
            if (!string.IsNullOrWhiteSpace(enrollmentStatus))
            {
                enrollmentQuery = enrollmentQuery.Where(e => e.EnrollmentStatus == enrollmentStatus);
            }

            // جلب البيانات إلى الذاكرة قبل تطبيق الفلاتر النصية
            var enrollments = await enrollmentQuery.ToListAsync();

            // تنفيذ الفلترة النصية على مستوى الذاكرة
            if (!string.IsNullOrWhiteSpace(studentName))
            {
                studentName = NormalizeArabicText(studentName);
                enrollments = enrollments
                    .Where(e => NormalizeArabicText(e.StudentName).Contains(studentName))
                    .ToList();
            }

            if (!string.IsNullOrWhiteSpace(courseName))
            {
                courseName = NormalizeArabicText(courseName);
                enrollments = enrollments
                    .Where(e => NormalizeArabicText(e.CourseName).Contains(courseName))
                    .ToList();
            }

            if (!string.IsNullOrWhiteSpace(semester))
            {
                semester = NormalizeArabicText(semester);
                enrollments = enrollments
                    .Where(e => NormalizeArabicText(e.Semester).Contains(semester))
                    .ToList();
            }

            return enrollments;
        }

        private string NormalizeArabicText(string text)
        {
            return text.Replace('أ', 'ا')
                       .Replace('إ', 'ا')
                       .Replace('آ', 'ا')
                       .Replace('ى', 'ي')
                       .Replace('ه', 'ة');
        }

        public async Task<IEnumerable<string>> GetAllEnrollmentsStatusesAsync()
        {
            var status = await _dbContext.Enrollments
               .Select(d => d.IsCompleted)
               .Distinct()
               .ToListAsync();

            return status;
        }

        public async Task<IEnumerable<string>> GetAllEnrollmentsSemsterAsync()
        {
            var status = await _dbContext.Enrollments
               .Select(d => d.Semester)
               .Distinct()
               .ToListAsync();

            return status;
        }
    }
}