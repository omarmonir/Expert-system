using FacultyManagementSystemAPI.Data;
using FacultyManagementSystemAPI.Models.DTOs.Courses;
using FacultyManagementSystemAPI.Models.DTOs.Student;
using FacultyManagementSystemAPI.Models.Entities;
using FacultyManagementSystemAPI.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;
namespace FacultyManagementSystemAPI.Repositories.Implementes
{
	public class StudentRepository(AppDbContext dbContext) : GenericRepository<Student>(dbContext), IStudentRepository
	{
		private readonly AppDbContext _dbContext = dbContext;

		public async Task<IEnumerable<StudentDto>> GetAllWithDepartmentNameAsync()
		{
			return await _dbContext.Students
				 .AsNoTrackingWithIdentityResolution()
				 .Select(s => new StudentDto
				 {
					 Id = s.Id,
					 Name = s.Name,
					 Email = s.Email,
					 Address = s.Address,
					 EnrollmentDate = s.EnrollmentDate,
					 Phone = s.Phone,
					 Gender = s.Gender,
					 NationalId = s.NationalId,
					 Nationality = s.Nationality,
					 Semester = s.Semester,
					 status = s.status,
					 StudentLevel = s.StudentLevel,
                     GPA_Average = s.GPA_Average,
					 High_School_degree = s.High_School_degree,
					 High_School_Section = s.High_School_Section,
					 CreditsCompleted = s.CreditsCompleted,
					 ImagePath = s.ImagePath,
					 DepartmentName = s.Department.Name
				 }).ToListAsync();
		}

		public async Task<StudentDto> GetByIdWithDepartmentNameAsync(int id)
		{
			return await _dbContext.Students
				 .AsNoTrackingWithIdentityResolution()
				 .Where(s => s.Id == id)
				 .Select(s => new StudentDto
				 {
                     Id = s.Id,
                     Name = s.Name,
                     Email = s.Email,
                     Address = s.Address,
                     EnrollmentDate = s.EnrollmentDate,
                     Phone = s.Phone,
                     Gender = s.Gender,
                     NationalId = s.NationalId,
                     Nationality = s.Nationality,
                     Semester = s.Semester,
                     status = s.status,
                     StudentLevel = s.StudentLevel,
                     GPA_Average = s.GPA_Average,
                     High_School_degree = s.High_School_degree,
                     High_School_Section = s.High_School_Section,
                     CreditsCompleted = s.CreditsCompleted,
                     ImagePath = s.ImagePath,
                     DepartmentName = s.Department.Name
                 }).FirstOrDefaultAsync();
		}

		public async Task<IEnumerable<StudentDto>> GetByNameWithDepartmentNameAsync(string name)
		{
			return await _dbContext.Students
				 .AsNoTrackingWithIdentityResolution()
				 .Where(s => s.Name.Contains(name))
				 .Select(s => new StudentDto
				 {
                     Id = s.Id,
                     Name = s.Name,
                     Email = s.Email,
                     Address = s.Address,
                     EnrollmentDate = s.EnrollmentDate,
                     Phone = s.Phone,
                     Gender = s.Gender,
                     NationalId = s.NationalId,
                     Nationality = s.Nationality,
                     Semester = s.Semester,
                     status = s.status,
                     StudentLevel = s.StudentLevel,
                     GPA_Average = s.GPA_Average,
                     High_School_degree = s.High_School_degree,
                     High_School_Section = s.High_School_Section,
                     CreditsCompleted = s.CreditsCompleted,
                     ImagePath = s.ImagePath,
                     DepartmentName = s.Department.Name
                 }).ToListAsync();
		}

		public async Task<bool> DepartmentExistsAsync(int departmentId)
		{
			return await _dbContext.Departments
				 .AnyAsync(d => d.Id == departmentId);
		}

		public async Task<bool> PhoneExistsAsync(string phoneNumber)
		{
			return await _dbContext.Students
				.AnyAsync(p => p.Phone == phoneNumber);
		}

		public async Task<bool> EmailExistsAsync(string email)
		{
			return await _dbContext.Students
				 .AnyAsync(s => s.Email.ToUpper() == email.ToUpper());
		}
		public async Task<StudentWithGradesDto> GetByIdWithHisGradeAsync(int id)
		{
			return await _dbContext.Enrollments
				 .AsNoTrackingWithIdentityResolution()
				 .Where(s => s.StudentId == id)
				 .Select(s => new StudentWithGradesDto
				 {
					 Id = s.Student.Id,
					 Name = s.Student.Name,
                     GPA_Average = s.Student.GPA_Average,
					 Exam1Grade = s.Exam1Grade,
					 Exam2Grade = s.Exam2Grade,
					 FinalGrade = s.FinalGrade,
					 Grade = s.Grade
				 }).FirstOrDefaultAsync();
		}

        public async Task<IEnumerable<StudentCountDto>> GetStudentCountByDepartmentAsync(int departmentId)
        {
            return await _dbContext.Students
             .Where(s => s.DepartmentId == departmentId)
             .GroupBy(s => s.Department.Name)
             .Select(g => new StudentCountDto
             {
                 DepartmentName = g.Key,
                 StudentCount = g.Count()
             })
             .ToListAsync();
        }
        public async Task<IEnumerable<CourseStudentCountDto>> GetCourseStudentCount(int courseId)
        {
            return await _dbContext.Enrollments
               .Where(e => e.CourseId == courseId)
               .GroupBy(e => new { e.CourseId, e.Course.Name })
               .Select(g => new CourseStudentCountDto
               {
                   CourseId = g.Key.CourseId,
                   CourseName = g.Key.Name,
                   StudentCount = g.Count()
               })
               .ToListAsync();
        }
    }
}
