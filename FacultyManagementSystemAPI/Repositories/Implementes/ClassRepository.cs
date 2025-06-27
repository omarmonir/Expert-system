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
        public async Task<bool> IsTimeAndLocationConflictAsync(TimeSpan startTime, TimeSpan endTime, string day, string location, int? excludeClassId = null)
        {
            var query = _dbSet.Where(c =>
                c.Day == day &&
                c.Location == location &&
                // التحقق من تداخل الأوقات
                (c.StartTime < endTime && c.EndTime > startTime)
            );

            // استبعاد المحاضرة الحالية في حالة التعديل
            if (excludeClassId.HasValue)
            {
                query = query.Where(c => c.Id != excludeClassId.Value);
            }

            return await query.AnyAsync();
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
                .OrderBy(c => c.Course.Semester) // ترتيب حسب الفرقة (السمستر)
                .ThenBy(c => c.Id) // ترتيب ثانوي حسب الـ ID
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
                1 or 2 => "سنة أولى",      // بدون "الـ"
                3 or 4 => "سنة ثانية",     // بدون "الـ"
                5 or 6 => "سنة ثالثة",     // بدون "الـ"
                7 or 8 => "سنة رابعة",     // بدون "الـ"
                _ => "غير معروف"
            };
        }
        public async Task<IEnumerable<ClassDto>> GetAllClassesWithProfNameAndCourseNameAsyncOptimized(
        string? divisionName = null,
        string? level = null)
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

            // فلترة بالـ Level عن طريق تحويل الـ Level للسمستر المطابق
            if (!string.IsNullOrEmpty(level))
            {
                var semesters = GetSemestersForLevel(level);
                if (semesters.Any())
                {
                    query = query.Where(c => semesters.Contains(c.Course.Semester));
                }
            }

            var classDtos = await query
                .OrderBy(c => c.Course.Semester) // ترتيب حسب الفرقة (السمستر)
                .ThenBy(c => c.Course.Name) // ترتيب ثانوي حسب اسم المادة
                .ThenBy(c => c.Id) // ترتيب ثالث حسب الـ ID
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
        private List<byte> GetSemestersForLevel(string level)
        {
            return level switch
            {
                "سنة أولى" => new List<byte> { 1, 2 },   // بدون "الـ"
                "سنة ثانية" => new List<byte> { 3, 4 },  // بدون "الـ"
                "سنة ثالثة" => new List<byte> { 5, 6 },  // بدون "الـ"
                "سنة رابعة" => new List<byte> { 7, 8 },  // بدون "الـ"
                _ => new List<byte>()
            };
        }
        public async Task<IEnumerable<ClassDto>> GetProfessorClassesAsync(int professorId)
        {
            var query = _dbContext.Classes
                .Where(c => c.ProfessorId == professorId)
                .Include(c => c.Professor)
                .Include(c => c.Course)
                    .ThenInclude(c => c.CourseDivisions)
                        .ThenInclude(cd => cd.Division)
                .Include(c => c.Course)
                    
              ;

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
        public async Task<IEnumerable<ClassDto>> GetStudentClassesAsync(int studentId)
        {
            var student = await _dbContext.Students
                .Include(s => s.Enrollments)
                    .ThenInclude(e => e.Course)
                        .ThenInclude(c => c.Classes)
                            .ThenInclude(cl => cl.Professor)
                .FirstOrDefaultAsync(s => s.Id == studentId);
            if (student == null)
                throw new KeyNotFoundException("الطالب غير موجود");
            var currentSemester = student.Semester;

            var classes = student.Enrollments
                .Where(e => e.NumberOFSemster == currentSemester && e.Course.Classes != null)
                .SelectMany(e => e.Course.Classes)
                .Select(c => new ClassDto
                {
                    Id = c.Id,
                    StartTime = c.StartTime,
                    EndTime = c.EndTime,
                    Day = c.Day,
                    Location = c.Location,
                    CourseName = c.Course.Name,
                    ProfessorName = c.Professor.FullName,
                    DivisionName = c.Course.CourseDivisions != null && c.Course.CourseDivisions.Any()
                        ? string.Join("، ", c.Course.CourseDivisions.Select(cd => cd.Division.Name))
                        : "لا يوجد شُعب مسجّلة",
                    Level = GetLevelFromSemester(c.Course.Semester)
                })
                .ToList();

            return classes;
        }

    }

}
