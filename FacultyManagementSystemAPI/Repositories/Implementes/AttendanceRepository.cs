using FacultyManagementSystemAPI.Data;
using FacultyManagementSystemAPI.Models.DTOs.Attendance;
using FacultyManagementSystemAPI.Models.Entities;
using FacultyManagementSystemAPI.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace FacultyManagementSystemAPI.Repositories.Implementes
{
    public class AttendanceRepository(AppDbContext dbContext) : GenericRepository<Attendance>(dbContext), IAttendanceRepository
    {
        private readonly AppDbContext _dbContext = dbContext;

        public async Task<IEnumerable<AttendanceDto>> GetAllAttendancesAsync(int pageNumber)
        {
            int pageSize = 20;

            var attendances =  _dbContext.Attendances
                .AsNoTracking()
                .Select(a => new AttendanceDto
                {
                    Id = a.Id,
                    Date = a.Date,
                    Status = a.Status,
                    StudentId = a.StudentId,
                    ClassesId = a.ClassesId,
                    StudentName = a.Student.Name,
                    CourseName = a.Class.Course.Name
                })
                ;
            int totalCount = await attendances.CountAsync();

            var pagedData = await attendances
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return pagedData;
        }

        public async Task<AttendanceDto> GetAttendanceByIdAsync(int id)
        {
            // جلب سجل الحضور
            var attendance = await _dbContext.Attendances
                .Where(a => a.Id == id)
                .FirstOrDefaultAsync();

            // جلب اسم الطالب
            var studentName = await _dbContext.Students
                .Where(s => s.Id == attendance.StudentId)
                .Select(s => s.Name)
                .FirstOrDefaultAsync();

            // جلب اسم الكورس
            var courseName = await _dbContext.Classes
                .Where(c => c.Id == attendance.ClassesId)
                .Select(c => c.Course.Name)
                .FirstOrDefaultAsync();

            // إنشاء الكائن النهائي
            return new AttendanceDto
            {
                Id = attendance.Id,
                Date = attendance.Date,
                Status = attendance.Status,
                StudentId = attendance.StudentId,
                ClassesId = attendance.ClassesId,
                StudentName = studentName,
                CourseName = courseName
            };
        }


        public async Task<IEnumerable<AttendanceDto>> GetAttendancesByClassIdAsync(int classId, int pageNumber)
        {
            int pageSize = 20;
            var attendances =  _dbContext.Attendances
                .Where(a => a.ClassesId == classId)
                .Select(a => new AttendanceDto
                {
                    Id = a.Id,
                    Date = a.Date,
                    Status = a.Status,
                    StudentId = a.StudentId,
                    ClassesId = a.ClassesId,
                    StudentName = a.Student.Name,
                    CourseName = a.Class.Course.Name
                })
                ;

            int totalCount = await attendances.CountAsync();

            var pagedData = await attendances
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return pagedData;
        }

        public async Task<IEnumerable<AttendanceDto>> GetAttendancesByStudentIdAsync(int studentId, int pageNumber)
        {
            int pageSize = 20;
            var attendances =  _dbContext.Attendances
                .Where(a => a.StudentId == studentId)
                .Select(a => new AttendanceDto
                {
                    Id = a.Id,
                    Date = a.Date,
                    Status = a.Status,
                    StudentId = a.StudentId,
                    ClassesId = a.ClassesId,
                    StudentName = a.Student.Name,
                    CourseName = a.Class.Course.Name
                })
                ;

            int totalCount = await attendances.CountAsync();

            var pagedData = await attendances
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return pagedData;
        }

        public async Task<bool> StudentExistsAsync(int studentId)
        {
            return await _dbContext.Students.AnyAsync(s => s.Id == studentId);
        }

        public async Task<bool> ClassExistsAsync(int classId)
        {
            return await _dbContext.Classes.AnyAsync(c => c.Id == classId);
        }

        public async Task<int> CountAttendanceAsync()
        {
            return await _dbContext.Attendances.CountAsync(a => a.Status == true);
        }

        public async Task<int> CountNoAttendanceAsync()
        {
            return await _dbContext.Attendances.CountAsync(a => a.Status == false);
        }
        public async Task<Course> GetCourseByNameAsync(string name)
        {
            return await _dbContext.Courses.FirstOrDefaultAsync(c => c.Name == name);
        }

        public async Task<Class> GetClassByProfessorAndCourseAsync(int professorId, int courseId)
        {
            return await _dbContext.Classes
                .FirstOrDefaultAsync(c => c.ProfessorId == professorId && c.CourseId == courseId);
        }

    }
}
