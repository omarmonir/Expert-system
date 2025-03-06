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

        public async Task<IEnumerable<ProfessorDto>> GetAllProfessorsAsync()
        {
            return await _context.Professors
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
                }).ToListAsync();
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
                .Select(c => new CourseDto
                {
                    Id = c.Course.Id,
                    Name = c.Course.Name,
                    Credits = c.Course.Credits,
                    Description = c.Course.Description,
                    Status = c.Course.Status,
                    Semester = c.Course.Semester,
                    PreCourseName = c.Course.PreCourse.Name,
                    ProfessorName = c.Professor.FullName
                }).Distinct().ToListAsync();
        }

        public async Task<bool> ProfessorExistsAsync(string professorName)
        {
            return await _context.Professors
                 .AnyAsync(p => p.FullName == professorName);
        }
    }
}
