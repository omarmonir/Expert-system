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
                        CurrentEnrolledStudents = c.Course.CurrentEnrolledStudents,
                        MaxSeats = c.Course.MaxSeats,
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
                    CurrentEnrolledStudents = c.Course.CurrentEnrolledStudents,
                    MaxSeats = c.Course.MaxSeats,
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
                    CurrentEnrolledStudents = c.Course.CurrentEnrolledStudents,
                    MaxSeats = c.Course.MaxSeats,
                    PreCourseName = c.Course.PreCourse != null ? c.Course.PreCourse.Name : "لا يوجد مقرر مطلوب لهذا المقرر",
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
                   Semester = c.Course.Semester,
                   CurrentEnrolledStudents = c.Course.CurrentEnrolledStudents,
                   MaxSeats = c.Course.MaxSeats,
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
            var courseDto = await _dbContext.Classes
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
                CurrentEnrolledStudents = c.Course.CurrentEnrolledStudents,
                MaxSeats = c.Course.MaxSeats,
                PreCourseName = c.Course.PreCourse != null ? c.Course.PreCourse.Name : "لا يوجد مقرر مطلوب لهذا المقرر",
                ProfessorName = c.Professor.FullName
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
                .Where(c => c.Status == "متاح")
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
    }
}
