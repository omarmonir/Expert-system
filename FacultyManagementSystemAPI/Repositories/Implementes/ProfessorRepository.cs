using FacultyManagementSystemAPI.Data;
using FacultyManagementSystemAPI.Models.DTOs.Courses;
using FacultyManagementSystemAPI.Models.DTOs.professors;
using FacultyManagementSystemAPI.Models.Entities;
using FacultyManagementSystemAPI.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;
namespace FacultyManagementSystemAPI.Repositories.Implementes
{
    public class ProfessorRepository(AppDbContext context) : GenericRepository<Professor>(context), IProfessorRepository
    {
        private readonly AppDbContext _context = context;

        public async Task<IEnumerable<ProfessorDto>> GetAllProfessorsAsync(int pageNumber)
        {
            int pageSize = 20;
            var query = _context.Professors
                .AsNoTracking()
                .Select(p => new ProfessorDto
                {
                    Id = p.Id,
                    FullName = p.FullName,
                    NationalId = p.NationalId,
                    Gender = p.Gender,
                    DateOfBirth = p.DateOfBirth,
                    Address = p.Address,
                    Email = p.Email,
                    Phone = p.Phone,
                    JoinDate = p.Join_Date,
                    Position = p.Position,
                    ImagePath = p.ImagePath,
                    DepartmentName = p.Department.Name
                });
            int totalCount = await query.CountAsync();

            var pagedData = await query
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return pagedData;
        }

        public async Task<ProfessorDto?> GetProfessorByIdAsync(int id)
        {
            return await _context.Professors
                .Where(p => p.Id == id)
                .Select(p => new ProfessorDto
                {
                    Id = p.Id,
                    FullName = p.FullName,
                    NationalId = p.NationalId,
                    Gender = p.Gender,
                    DateOfBirth = p.DateOfBirth,
                    Address = p.Address,
                    Email = p.Email,
                    Phone = p.Phone,
                    JoinDate = p.Join_Date,
                    Position = p.Position,
                    ImagePath = p.ImagePath,
                    DepartmentName = p.Department.Name
                }).FirstOrDefaultAsync();
        }

        public async Task<IEnumerable<ProfessorDto>> GetByDepartmentIdAsync(int departmentId)
        {
            return await _context.Professors
                .Where(p => p.DepartmentId == departmentId)
                .Select(p => new ProfessorDto
                {
                    Id = p.Id,
                    FullName = p.FullName,
                    NationalId = p.NationalId,
                    Gender = p.Gender,
                    DateOfBirth = p.DateOfBirth,
                    Address = p.Address,
                    Email = p.Email,
                    Phone = p.Phone,
                    JoinDate = p.Join_Date,
                    Position = p.Position,
                    ImagePath = p.ImagePath,
                    DepartmentName = p.Department.Name
                }).ToListAsync();
        }

        public async Task<IEnumerable<CourseDto>> GetCoursesByProfessorIdAsync(int professorId)
        {
            return await _context.Classes
                .Where(c => c.ProfessorId == professorId)
                .Select(c => c.Course) // select the course
                .Distinct()
                .Select(course => new CourseDto
                {
                    Id = course.Id,
                    Name = course.Name,
                    Credits = course.Credits,
                    Description = course.Description,
                    Status = course.Status,
                    Semester = course.Semester,
                    Code = course.Code,
                    MaxSeats = course.MaxSeats,
                    CurrentEnrolledStudents = course.CurrentEnrolledStudents,
                    PreCourseName = course.Prerequisites.Any()
                        ? string.Join("، ", course.Prerequisites.Select(p => p.PrerequisiteCourse.Name))
                        : "لا يوجد مقرر مطلوب لهذا المقرر",
                    ProfessorName = course.Classes
                        .Where(cls => cls.ProfessorId == professorId)
                        .Select(cls => cls.Professor.FullName)
                        .FirstOrDefault(),
                    DepartmentName = course.Department.Name
                })
                .ToListAsync();
        }


        public async Task<bool> ProfessorExistsAsync(string professorName)
        {
            return await _context.Professors
                 .AnyAsync(p => p.FullName == professorName);
        }

        public async Task<IEnumerable<string>> GetAllProfessorsNameAsync()
        {
            var names = await _context.Professors
          .Select(d => d.FullName)
          .ToListAsync();

            return names;
        }

        public async Task<Department?> GetDepartmentByNameAsync(string name)
        {
            return await _context.Departments
                .FirstOrDefaultAsync(d => d.Name.ToLower() == name.ToLower());
        }
    }
}
