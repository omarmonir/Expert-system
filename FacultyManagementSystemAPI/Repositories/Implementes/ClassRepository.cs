using FacultyManagementSystemAPI.Data;
using FacultyManagementSystemAPI.Models.DTOs.Classes;
using FacultyManagementSystemAPI.Models.DTOs.Courses;
using FacultyManagementSystemAPI.Models.Entities;
using FacultyManagementSystemAPI.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace FacultyManagementSystemAPI.Repositories.Implementes
{
    public class ClassRepository : GenericRepository<Class>, IClassRepository
    {
        private readonly AppDbContext _dbContext;
        private readonly DbSet<Class> _dbSet;

        public ClassRepository(AppDbContext dbContext) : base(dbContext)
        {
            _dbContext = dbContext;
            _dbSet = _dbContext.Set<Class>();
        }

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

        public async Task<ClassDto> GetClassByIdAsync(int courseId)
        {
            //return await _dbContext.Courses.FindAsync(courseId);

            var courseDto = await _dbContext.Classes
           .AsNoTrackingWithIdentityResolution()
           .Where(c => c.Id == courseId)
           .Select(c => new ClassDto
           {
               Id = c.Id,
               StartTime = c.StartTime,
               EndTime = c.EndTime,
               Day = c.Day,
               Location = c.Location,

               CourseName = c.Course.PreCourse != null ? c.Course.PreCourse.Name : "لا يوجد مقرر مطلوب لهذا المقرر",
               ProfessorName = c.Professor.FullName
           })

               .FirstOrDefaultAsync();
            return courseDto;
        }

        public async Task<Professor> GetProfessorByNameAsync(string professorName)
        {
            return await _dbContext.Professors.FirstOrDefaultAsync(p => p.FullName == professorName);
        }


        public async Task<bool> IsCourseAlreadyAssignedAsync(int courseId, int professorId)
        {
            return await _dbContext.Classes.AnyAsync(c => c.CourseId == courseId && c.ProfessorId == professorId);
        }

        public async Task<bool> ClassExistsAsync(int? ClassId)
        {
            return await _dbContext.Classes
                 .AnyAsync(d => d.Id == ClassId);
        }
        public async Task<bool> IsTimeAndLocationConflictAsync(TimeSpan startTime, TimeSpan endTime, string day, string location)
        {
            return await _dbSet.AnyAsync(c =>
                EF.Property<string>(c, "Day") == day &&
                EF.Property<string>(c, "Location") == location &&
                (EF.Property<TimeSpan>(c, "StartTime") < endTime && EF.Property<TimeSpan>(c, "EndTime") > startTime)
            );
        }

        public async Task<IEnumerable<ClassDto>> GetAllClassesWithProfNameAndCourseNameAsync()
        {
            return await _dbContext.Classes
           .AsNoTrackingWithIdentityResolution()

           .Select(c => new ClassDto
           {
               Id = c.Id,
               StartTime = c.StartTime,
               EndTime = c.EndTime,
               Day = c.Day,
               Location = c.Location,

               CourseName = c.Course.PreCourse != null ? c.Course.PreCourse.Name : "لا يوجد مقرر مطلوب لهذا المقرر",
               ProfessorName = c.Professor.FullName
           }).ToListAsync();

        }
        public async Task<int> CountAsync()
        {
            return await _dbContext.Classes.CountAsync();


        }

        public async Task<IEnumerable<string>> GetAllLocationsNameAsync()
        {
            var names = await _dbContext.Classes
         .Select(d => d.Location)
         .ToListAsync();

            return names;
        }
    }
}
