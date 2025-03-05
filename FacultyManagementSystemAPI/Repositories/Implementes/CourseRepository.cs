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
                    PreCourseName = c.Course.PreCourse != null ? c.Course.PreCourse.Name : "لا يوجد مقرر مطلوب لهذا المقرر",
                    ProfessorName = c.Professor.FullName,
                    MaxSeats = c.Course.MaxSeats,
                    CurrentEnrolledStudents = c.Course.CurrentEnrolledStudents
                })
                .ToListAsync();
        }
        public async Task<IEnumerable<CourseDto>> GetAllWithPreCourseNameAsync()
        {
            return await _dbContext.Classes
                    .AsNoTrackingWithIdentityResolution()
                    .Select(c => new CourseDto
                    {
                        Id = c.CourseId,
                        Name = c.Course.Name,
                        Description = c.Course.Description,
                        Credits = c.Course.Credits,
                        Status = c.Course.Status,
                        Semester = c.Course.Semester,
                        PreCourseName = c.Course.PreCourse != null ? c.Course.PreCourse.Name : "لا يوجد مقرر مطلوب لهذا المقرر",
                        ProfessorName = c.Professor.FullName,
                        MaxSeats = c.Course.MaxSeats,
                        CurrentEnrolledStudents = c.Course.CurrentEnrolledStudents
                    })

                                 .ToListAsync();
        }

        public async Task<IEnumerable<CourseDto>> GetCoursesBySemesterWithPreCourseNameAsync(byte semester)
        {
            return await _dbContext.Classes
                .AsNoTrackingWithIdentityResolution()
                .Where(c => c.Course.Semester == semester)
                .Select(c => new CourseDto
                {
                    Id = c.CourseId,
                    Name = c.Course.Name,
                    Description = c.Course.Description,
                    Credits = c.Course.Credits,
                    Status = c.Course.Status,
                    Semester = c.Course.Semester,
                    PreCourseName = c.Course.PreCourse != null ? c.Course.PreCourse.Name : "لا يوجد مقرر مطلوب لهذا المقرر",
                    ProfessorName = c.Professor.FullName,
                    MaxSeats = c.Course.MaxSeats,
                    CurrentEnrolledStudents = c.Course.CurrentEnrolledStudents
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
                    Id = c.CourseId,
                    Name = c.Course.Name,
                    Description = c.Course.Description,
                    Credits = c.Course.Credits,
                    Status = c.Course.Status,
                    Semester = c.Course.Semester,
                    PreCourseName = c.Course.PreCourse != null ? c.Course.PreCourse.Name : "لا يوجد مقرر مطلوب لهذا المقرر",
                    MaxSeats = c.Course.MaxSeats,
                    CurrentEnrolledStudents = c.Course.CurrentEnrolledStudents
                })
                .ToListAsync();
        }
        public async Task<IEnumerable<CourseDto>> GetCoursesByProfessorIdWithPreCourseNameAsync(int facultyId)
        {
            return await _dbContext.Classes
               .Where(c => c.ProfessorId == facultyId)
               .Select(c => new CourseDto
               {
                   Id = c.CourseId,
                   Name = c.Course.Name,
                   Description = c.Course.Description,
                   Credits = c.Course.Credits,
                   Status = c.Course.Status,
                   Semester = c.Course.Semester,
                   PreCourseName = c.Course.PreCourse != null ? c.Course.PreCourse.Name : "لا يوجد مقرر مطلوب لهذا المقرر",
                   ProfessorName = c.Professor.FullName,
                   MaxSeats = c.Course.MaxSeats,
                   CurrentEnrolledStudents = c.Course.CurrentEnrolledStudents
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
            return await _dbContext.Classes
                                .AsNoTrackingWithIdentityResolution()
                                .Where(c => c.CourseId == id)
                                .Select(c => new CourseDto
                                {
                                    Id = c.CourseId,
                                    Name = c.Course.Name,
                                    Description = c.Course.Description,
                                    Credits = c.Course.Credits,
                                    Status = c.Course.Status,
                                    Semester = c.Course.Semester,
                                    PreCourseName = c.Course.PreCourse != null ? c.Course.PreCourse.Name : "لا يوجد مقرر مطلوب لهذا المقرر",
                                    ProfessorName = c.Professor.FullName,
                                    MaxSeats = c.Course.MaxSeats,
                                    CurrentEnrolledStudents = c.Course.CurrentEnrolledStudents
                                })

                                 .FirstOrDefaultAsync();
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
                .Select(e => new CourseDto
                {
                    Id = e.Course.Id,
                    Name = e.Course.Name,
                    Description = e.Course.Description,
                    Credits = e.Course.Credits,
                    Status = e.Course.Status,
                    Semester = e.Course.Semester,
                    MaxSeats = e.Course.MaxSeats,
                    PreCourseName = e.Course.PreCourse.Name
                 
                })
                .Distinct()
                .ToListAsync();
        }
    }
}
