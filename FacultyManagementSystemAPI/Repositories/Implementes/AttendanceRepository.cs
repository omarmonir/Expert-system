using FacultyManagementSystemAPI.Data;
using FacultyManagementSystemAPI.Models.DTOs.Attendance;
using FacultyManagementSystemAPI.Models.Entities;
using FacultyManagementSystemAPI.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace FacultyManagementSystemAPI.Repositories.Implementes
{
    public class AttendanceRepository(AppDbContext dbContext) : GenericRepository<Attendance>(dbContext), IAttendanceRepository
    {
        private readonly AppDbContext _dbContext = dbContext;

        public async Task<IEnumerable<AttendanceDto>> GetAllAttendancesAsync()
        {
            var attendances = await _dbContext.Attendances
                .AsNoTrackingWithIdentityResolution()
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
                .ToListAsync();

            return attendances;
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


        public async Task<IEnumerable<AttendanceDto>> GetAttendancesByClassIdAsync(int classId)
        {
            var attendances = await _dbContext.Attendances
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
                .ToListAsync();

            return attendances;
        }

        public async Task<IEnumerable<AttendanceDto>> GetAttendancesByStudentIdAsync(int studentId)
        {
            var attendances = await _dbContext.Attendances
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
                .ToListAsync();

            return attendances;
        }
    }
}
