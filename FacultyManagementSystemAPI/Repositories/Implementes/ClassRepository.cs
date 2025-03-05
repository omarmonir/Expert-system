using FacultyManagementSystemAPI.Data;
using FacultyManagementSystemAPI.Models.DTOs.Classes;
using FacultyManagementSystemAPI.Models.Entities;
using FacultyManagementSystemAPI.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace FacultyManagementSystemAPI.Repositories.Implementes
{
    public class ClassRepository(AppDbContext dbContext) : GenericRepository<Class>(dbContext), IClassRepository
    {
        private readonly AppDbContext _dbContext = dbContext;
        public async Task<IEnumerable<ClassDto>> GetAllClassesAsync()
        {
            var classes = await _dbContext.Classes
           .Include(c => c.Professor)
           .Include(c => c.Course)
           .Select(c => new ClassDto
           {
               Id = c.Id,
               StartTime = c.StartTime,
               EndTime = c.EndTime,
               Day = c.Day,
               Location = c.Location,
               ProfessorName = c.Professor.FullName,
               CourseName = c.Course.Name
           })
           .ToListAsync();

            return classes;
        }

        public async Task<IEnumerable<ClassDto>> GetClassesByProfessorIdAsync(int professorId)
        {
            var classes = await _dbContext.Classes
           .Where(c => c.ProfessorId == professorId)
           .Include(c => c.Course)
           .Select(c => new ClassDto
           {
               Id = c.Id,
               StartTime = c.StartTime,
               EndTime = c.EndTime,
               Day = c.Day,
               Location = c.Location,
               CourseName = c.Course.Name,
               ProfessorName = c.Professor.FullName

           })
           .ToListAsync();

            return classes;
        }

     

    }
}
