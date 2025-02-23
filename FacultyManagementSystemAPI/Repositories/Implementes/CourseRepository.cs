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
                    ProfessorName = c.Professor.FullName
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
                        ProfessorName = c.Professor.FullName
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
                    ProfessorName = c.Professor.FullName
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
                    Semester = c.Course.Semester,
                    PreCourseName = c.Course.PreCourse != null ? c.Course.PreCourse.Name : "لا يوجد مقرر مطلوب لهذا المقرر",
                })
                .ToListAsync();
        }
        public async Task<IEnumerable<CourseDto>> GetCoursesByProfessorIdWithPreCourseNameAsync(int facultyId)
        {
            return await _dbContext.Classes
               .Where(c => c.ProfessorId == facultyId)
               .Select(c => new CourseDto
               {
                   Id = c.Id,
                   Name = c.Course.Name,
                   Description = c.Course.Description,
                   Credits = c.Course.Credits,
                   Status = c.Course.Status,
                   Semester = c.Course.Semester,
                   PreCourseName = c.Course.PreCourse != null ? c.Course.PreCourse.Name : "لا يوجد مقرر مطلوب لهذا المقرر",
                   ProfessorName = c.Professor.FullName
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
                                    ProfessorName = c.Professor.FullName
                                })

                                 .FirstOrDefaultAsync();
        }
    }
}
