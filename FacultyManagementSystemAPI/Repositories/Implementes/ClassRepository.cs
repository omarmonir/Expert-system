using FacultyManagementSystemAPI.Data;
using FacultyManagementSystemAPI.Models.Entities;
using FacultyManagementSystemAPI.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace FacultyManagementSystemAPI.Repositories.Implementes
{
    public class ClassRepository(AppDbContext dbContext) : GenericRepository<Class>(dbContext), IClassRepository
    {
        private readonly AppDbContext _dbContext;

        public async Task<Class> GetClassByProfessorNameAsync(string professorName)
        {
            return await _dbContext.Classes
                .Include(c => c.Professor)
                .FirstOrDefaultAsync(c => c.Professor.FullName == professorName);
        }

        public async Task UpdateClassAsync(Class classEntity)
        {
            _dbContext.Classes.Update(classEntity);
            await _dbContext.SaveChangesAsync();
        }

        public async Task AssignCourseToProfessorAsync(int courseId, string professorName)
        {
            // ابحث عن الكورس والأستاذ
            var course = await _dbContext.Courses.FindAsync(courseId);
            var professor = await _dbContext.Professors
                                          .FirstOrDefaultAsync(p => p.FullName == professorName);

            // تعيين الكورس للأستاذ
            var newClass = new Class
            {
                CourseId = courseId,
                ProfessorId = professor.Id
            };
            try
            {
                _dbContext.Classes.Add(newClass);
                await _dbContext.SaveChangesAsync();
            }
            catch (DbUpdateException ex)
            {
                var innerException = ex.InnerException?.Message ?? ex.Message;
                throw new Exception($"حدث خطأ أثناء حفظ التغييرات في قاعدة البيانات: {innerException}");
            }

        }

        public async Task<Course> GetCourseByIdAsync(int courseId)
        {
            return await _dbContext.Courses.FindAsync(courseId);
        }

        public async Task<Professor> GetProfessorByNameAsync(string professorName)
        {
            return await _dbContext.Professors.FirstOrDefaultAsync(p => p.FullName == professorName);
        }


        public async Task<bool> IsCourseAlreadyAssignedAsync(int courseId, int professorId)
        {
            return await _dbContext.Classes.AnyAsync(c => c.CourseId == courseId && c.ProfessorId == professorId);
        }
    }
}
