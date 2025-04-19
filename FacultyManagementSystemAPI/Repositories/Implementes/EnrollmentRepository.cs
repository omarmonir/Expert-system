using FacultyManagementSystemAPI.Data;
using FacultyManagementSystemAPI.Models.DTOs.Enrollment;
using FacultyManagementSystemAPI.Models.Entities;
using FacultyManagementSystemAPI.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace FacultyManagementSystemAPI.Repositories.Implementes
{
    public class EnrollmentRepository : GenericRepository<Enrollment>, IEnrollmentRepository
    {
        private readonly AppDbContext _dbContext;

        public EnrollmentRepository(AppDbContext dbContext) : base(dbContext)
        {
            _dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
        }

        public async Task<IEnumerable<EnrollmentDto>> GetAllIncludeStudentNameCourseNameAsync()
        {
            return await _dbContext.Enrollments
                .AsNoTracking()
                .Select(e => new EnrollmentDto
                {
                    Id = e.Id,
                    StudentID = e.StudentId,
                    StudentName = e.Student.Name ?? "غير معروف",
                    CourseCode = e.Course.Code ?? "غير معروف",
                    CourseName = e.Course.Name ?? "غير معروف",
                    EnrollmentDate = e.AddedEnrollmentDate,
                    EnrollmentStatus = e.IsCompleted,
                    Semester = e.Semester
                })
                .ToListAsync();
        }

        public async Task<EnrollmentDto> GetByIdIncludeStudentNameCourseNameAsync(int id)
        {
            return await _dbContext.Enrollments
                .AsNoTracking()
                .Where(e => e.Id == id)
                .Select(e => new EnrollmentDto
                {
                    Id = e.Id,
                    StudentID = e.StudentId,
                    StudentName = e.Student.Name ?? "غير معروف",
                    CourseCode = e.Course.Code ?? "غير معروف",
                    CourseName = e.Course.Name ?? "غير معروف",
                    EnrollmentDate = e.AddedEnrollmentDate,
                    EnrollmentStatus = e.IsCompleted,
                    Semester = e.Semester
                })
                .FirstOrDefaultAsync();
        }

        public async Task<IEnumerable<EnrollmentDto>> GetBySemesterAsync(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentException("يجب تحديد اسم الفصل الدراسي", nameof(name));

            // استخدام طريقة EF.Functions.Like للبحث بشكل أفضل
            return await _dbContext.Enrollments
                .Where(e => EF.Functions.Like(e.Semester, $"%{name}%"))
                .AsNoTracking()
                .Select(e => new EnrollmentDto
                {
                    Id = e.Id,
                    StudentID = e.StudentId,
                    StudentName = e.Student.Name ?? "غير معروف",
                    CourseCode = e.Course.Code ?? "غير معروف",
                    CourseName = e.Course.Name ?? "غير معروف",
                    EnrollmentDate = e.AddedEnrollmentDate,
                    EnrollmentStatus = e.IsCompleted,
                    Semester = e.Semester
                })
                .ToListAsync();
        }

        public async Task<IEnumerable<EnrollmentDto>> GetByStudentIdAsync(int studentId)
        {
            if (studentId <= 0)
                throw new ArgumentException("رقم الطالب يجب أن يكون أكبر من صفر", nameof(studentId));

            return await _dbContext.Enrollments
                .AsNoTracking()
                .Where(e => e.StudentId == studentId)
                .Select(e => new EnrollmentDto
                {
                    Id = e.Id,
                    StudentID = e.StudentId,
                    StudentName = e.Student.Name ?? "غير معروف",
                    CourseCode = e.Course.Code ?? "غير معروف",
                    CourseName = e.Course.Name ?? "غير معروف",
                    EnrollmentDate = e.AddedEnrollmentDate,
                    EnrollmentStatus = e.IsCompleted,
                    Semester = e.Semester
                })
                .ToListAsync();
        }

        public async Task<IEnumerable<EnrollmentDto>> GetByCourseIdAsync(int courseId)
        {
            if (courseId <= 0)
                throw new ArgumentException("رقم المقرر يجب أن يكون أكبر من صفر", nameof(courseId));

            return await _dbContext.Enrollments
                .AsNoTracking()
                .Where(e => e.CourseId == courseId)
                .Select(e => new EnrollmentDto
                {
                    Id = e.Id,
                    StudentID = e.StudentId,
                    StudentName = e.Student.Name ?? "غير معروف",
                    CourseCode = e.Course.Code ?? "غير معروف",
                    CourseName = e.Course.Name ?? "غير معروف",
                    EnrollmentDate = e.AddedEnrollmentDate,
                    EnrollmentStatus = e.IsCompleted,
                    Semester = e.Semester
                })
                .ToListAsync();
        }

        public async Task<int> CountAsync() => await _dbContext.Enrollments.CountAsync();

        public async Task<int> GetCompletedEnrollmentCountAsync() =>
            await _dbContext.Enrollments.CountAsync(e => e.IsCompleted == "ناجح");

        public async Task<int> GetAllWaitEnrollmentStudentsCountAsync() =>
            await _dbContext.Enrollments.CountAsync(e => e.IsCompleted == "قيد الدراسة");

        public async Task<int> CountDeletedAsync() =>
            await _dbContext.Enrollments.CountAsync(e => e.DeletedEnrollmentDate != null);

        public async Task<int> GetAllEnrollmentStudentsCountAsync() =>
            await _dbContext.Enrollments
                .Select(e => e.StudentId)
                .Distinct()
                .CountAsync();

        public async Task<Enrollment> GetByStudentAndCourseIdAsync(int studentId, int courseId)
        {
            if (studentId <= 0)
                throw new ArgumentException("رقم الطالب يجب أن يكون أكبر من صفر", nameof(studentId));

            if (courseId <= 0)
                throw new ArgumentException("رقم المقرر يجب أن يكون أكبر من صفر", nameof(courseId));

            return await _dbContext.Enrollments
                .FirstOrDefaultAsync(e => e.StudentId == studentId && e.CourseId == courseId);
        }

        public async Task UpdateAsync(Enrollment enrollment)
        {
            if (enrollment == null)
                throw new ArgumentNullException(nameof(enrollment));

            _dbContext.Enrollments.Update(enrollment);
            await _dbContext.SaveChangesAsync();
        }

        public async Task<IEnumerable<EnrollmentDto>> GetFilteredEnrollmentsAsync(
            string? studentName, string? courseName, string? enrollmentStatus, string? semester)
        {
            var query = _dbContext.Enrollments.AsNoTracking();

            // تطبيق الفلاتر على مستوى قاعدة البيانات حيثما أمكن
            if (!string.IsNullOrWhiteSpace(enrollmentStatus))
            {
                query = query.Where(e => e.IsCompleted == enrollmentStatus);
            }

            if (!string.IsNullOrWhiteSpace(semester))
            {
                var normalizedSemester = NormalizeArabicText(semester);
                query = query.Where(e => EF.Functions.Like(e.Semester, $"%{semester}%"));
            }

            if (!string.IsNullOrWhiteSpace(studentName))
            {
                var normalizedStudentName = NormalizeArabicText(studentName);
                query = query.Where(e => EF.Functions.Like(e.Student.Name, $"%{studentName}%"));
            }

            if (!string.IsNullOrWhiteSpace(courseName))
            {
                var normalizedCourseName = NormalizeArabicText(courseName);
                query = query.Where(e => EF.Functions.Like(e.Course.Name, $"%{courseName}%"));
            }

            // جلب البيانات مع تضمين العلاقات المطلوبة واستخدام التعيين المباشر
            return await query
                .Include(e => e.Student)
                .Include(e => e.Course)
                .Select(e => new EnrollmentDto
                {
                    Id = e.Id,
                    StudentID = e.StudentId,
                    StudentName = e.Student.Name ?? "غير معروف",
                    CourseCode = e.Course.Code ?? "غير معروف",
                    CourseName = e.Course.Name ?? "غير معروف",
                    EnrollmentDate = e.AddedEnrollmentDate,
                    EnrollmentStatus = e.IsCompleted,
                    Semester = e.Semester
                })
                .ToListAsync();
        }

        private string NormalizeArabicText(string text)
        {
            if (string.IsNullOrWhiteSpace(text))
                return string.Empty;

            return text.Replace('أ', 'ا')
                       .Replace('إ', 'ا')
                       .Replace('آ', 'ا')
                       .Replace('ى', 'ي')
                       .Replace('ة', 'ه');
        }

        public async Task<IEnumerable<string>> GetAllEnrollmentsStatusesAsync() =>
            await _dbContext.Enrollments
                .Select(d => d.IsCompleted)
                .Distinct()
                .ToListAsync();

        public async Task<IEnumerable<string>> GetAllEnrollmentsSemsterAsync() =>
            await _dbContext.Enrollments
                .Select(d => d.Semester)
                .Distinct()
                .ToListAsync();
    }
}