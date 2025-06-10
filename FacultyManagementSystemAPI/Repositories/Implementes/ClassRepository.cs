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

        public async Task<ClassDto> GetClassByIdAsync(int classId)
        {
            var courseDto = await _dbContext.Classes
                .Include(c => c.Professor)
                .Include(c => c.Course)
                    
                .AsNoTrackingWithIdentityResolution()
                .Where(c => c.Id == classId)
                .Select(c => new ClassDto
                {
                    Id = c.Id,
                    StartTime = c.StartTime,
                    EndTime = c.EndTime,
                    Day = c.Day,
                    Location = c.Location,
                    CourseName = c.Course.Name,
                    ProfessorName = c.Professor.FullName,
                   
                    DivisionName = c.Course.CourseDivisions.Any()
                        ? string.Join("، ", c.Course.CourseDivisions.Select(cd => cd.Division.Name))
                        : "لا يوجد شُعب مسجّلة",

                    Level = GetLevelFromSemester(c.Course.Semester)
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

        public async Task<IEnumerable<ClassDto>> GetAllClassesWithProfNameAndCourseNameAsync(int pageNumber)
        {
            const int pageSize = 20;

            var query = _dbContext.Classes
                .Include(c => c.Professor)
                .Include(c => c.Course)
                   

                .Include(c => c.Course)
                    .ThenInclude(co => co.CourseDivisions)
                        .ThenInclude(cd => cd.Division)
                .AsNoTrackingWithIdentityResolution()
                .OrderBy(c => c.Id)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize);

            return await query
                .Select(c => new ClassDto
                {
                    Id = c.Id,
                    StartTime = c.StartTime,
                    EndTime = c.EndTime,
                    Day = c.Day,
                    Location = c.Location,
                    CourseName = c.Course.Name,
                    ProfessorName = c.Professor.FullName,
                   
                    DivisionName = c.Course.CourseDivisions.Any()
                        ? string.Join("، ", c.Course.CourseDivisions.Select(cd => cd.Division.Name))
                        : "لا يوجد شُعب مسجّلة",
                    Level = GetLevelFromSemester(c.Course.Semester)
                })
                .ToListAsync();
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
        private static string GetLevelFromSemester(byte semester)
        {
            return semester switch
            {
                1 or 2 => "الفرقة الأولى",
                3 or 4 => "الفرقة الثانية",
                5 or 6 => "الفرقة الثالثة",
                7 or 8 => "الفرقة الرابعة",
                _ => "غير معروف"
            };
        }
        public async Task<IEnumerable<ClassDto>> GetAllClassesWithProfNameAndCourseNameAsync(   
                string? divisionName = null,
                 byte? semester = null)
        {
            var query = _dbContext.Classes
          .Include(c => c.Professor)
          .Include(c => c.Course)             
          .Include(c => c.Course)
              .ThenInclude(co => co.CourseDivisions)
                  .ThenInclude(cd => cd.Division)
          .AsNoTrackingWithIdentityResolution();
            if (!string.IsNullOrEmpty(divisionName))
            {
                query = query.Where(c => c.Course.CourseDivisions
                    .Any(cd => cd.Division.Name == divisionName));
            }

            if (semester.HasValue)
            {
                query = query.Where(c => c.Course.Semester == semester.Value);
            }
            var classDtos = await query
        .OrderBy(c => c.Id)
        .Select(c => new ClassDto
        {
            Id = c.Id,
            StartTime = c.StartTime,
            EndTime = c.EndTime,
            Day = c.Day,
            Location = c.Location,
            CourseName = c.Course.Name,
            ProfessorName = c.Professor.FullName,           
            DivisionName = string.Join("، ", c.Course.CourseDivisions.Select(cd => cd.Division.Name)),
            Level = GetLevelFromSemester(c.Course.Semester)
        })
        .ToListAsync();

            return classDtos;
        }
    }
}
