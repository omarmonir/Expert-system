using FacultyManagementSystemAPI.Data;
using FacultyManagementSystemAPI.Models.DTOs.Courses;
using FacultyManagementSystemAPI.Models.Entities;
using FacultyManagementSystemAPI.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Linq;

namespace FacultyManagementSystemAPI.Repositories.Implementes
{
	public class CourseRepository : GenericRepository<Course>, ICourseRepository
	{
		private readonly AppDbContext _dbContext;
		public CourseRepository(AppDbContext dbContext) : base(dbContext)
		{
			_dbContext = dbContext;
		}
		public async Task<IEnumerable<CourseDto>> SearchCoursesWithPreCourseNameAsync(string searchTerm)
		{
			return await _dbContext.Courses
				.AsNoTrackingWithIdentityResolution()
				.Where(c => c.Name.Contains(searchTerm))
				.Select(c => new CourseDto
				{
					Id = c.Id,
					Name = c.Name,
					Description = c.Description,
					Credits = c.Credits,
					Status = c.Status,
					Semester = c.Semester,
					PreCourseId = c.PreCourse.Id,
					PreCourseName = c.PreCourse != null ? c.PreCourse.Name : null
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
							Semester = c.Semester,
							PreCourseId = c.PreCourse.Id,
							PreCourseName = c.PreCourse != null ? c.PreCourse.Name : null
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
							Semester = c.Semester,
							PreCourseId = c.PreCourse.Id,
							PreCourseName = c.PreCourse != null ? c.PreCourse.Name : null
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
					PreCourseId = c.CourseId,
					PreCourseName = c.Course.PreCourse != null ? c.Course.PreCourse.Name : null
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
				   PreCourseId = c.CourseId,
				   PreCourseName = c.Course.PreCourse != null ? c.Course.PreCourse.Name : null,
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
			return await _dbContext.Courses
								.AsNoTrackingWithIdentityResolution()
								.Where(c => c.Id == id)
								.Select(c => new CourseDto
								{
									Id = c.Id,
									Name = c.Name,
									Description = c.Description,
									Credits = c.Credits,
									Status = c.Status,
									Semester = c.Semester,
									PreCourseId = c.PreCourseId,
									PreCourseName = c.PreCourse != null ? c.PreCourse.Name : null
								})

								 .FirstOrDefaultAsync();
		}
	}
}
