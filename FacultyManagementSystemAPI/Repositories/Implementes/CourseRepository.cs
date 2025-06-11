using FacultyManagementSystemAPI.Data;
using FacultyManagementSystemAPI.Models.DTOs.Courses;
using FacultyManagementSystemAPI.Models.DTOs.Student;
using FacultyManagementSystemAPI.Models.Entities;
using FacultyManagementSystemAPI.Repositories.Interfaces;
using iTextSharp.text;
using Microsoft.EntityFrameworkCore;

namespace FacultyManagementSystemAPI.Repositories.Implementes
{
    public class CourseRepository(AppDbContext dbContext) : GenericRepository<Course>(dbContext), ICourseRepository
    {

        private readonly AppDbContext _dbContext = dbContext;
        private const decimal PASSING_GRADE = 60;
        //    public async Task<IEnumerable<CourseDto>> SearchCoursesWithPreCourseNameAsync(string searchTerm)
        //    {


        //        return await _dbContext.Classes
        //            .AsNoTrackingWithIdentityResolution()
        //            .Where(c => c.Course.Name.Contains(searchTerm))
        //            .Select(c => new CourseDto
        //            {
        //                Id = c.CourseId,
        //                Name = c.Course.Name,
        //                Description = c.Course.Description,
        //                Credits = c.Course.Credits,
        //                Status = c.Course.Status,
        //                Semester = c.Course.Semester,
        //                Code = c.Course.Code,
        //                CurrentEnrolledStudents = _dbContext.Enrollments.Count(e => e.CourseId == c.Course.Id),
        //                MaxSeats = c.Course.MaxSeats,
        //                PreCourseName = c.Course.PreCourse != null ? c.Course.PreCourse.Name : "لا يوجد مقرر مطلوب لهذا المقرر",
        //                ProfessorName = c.Professor.FullName,
        //                DepartmentName = c.Course.CourseDepartments.Select(d => d.Department.Name).FirstOrDefault()??  "لا يوجد قسم لهذا المقرر"
        //            })
        //            .ToListAsync();
        //    }

        public async Task<IEnumerable<CourseDto>> GetAllWithPreCourseNameAsync(int pageNumber)
        {
            int pageSize = 20;
            var query = _dbContext.Courses
                 .Include(c => c.Prerequisites)
                     .ThenInclude(p => p.PrerequisiteCourse)
                 .Include(c => c.Classes)
                     .ThenInclude(cl => cl.Professor)
                 .Include(c => c.Department)
                 .AsNoTracking()
                 .Select(c => new CourseDto
                 {
                     Id = c.Id,
                     Name = c.Name,
                     Description = c.Description,
                     Credits = c.Credits,
                     Status = c.Status,
                     Code = c.Code,
                     CurrentEnrolledStudents = c.CurrentEnrolledStudents,
                     MaxSeats = c.MaxSeats,
                     Semester = c.Semester,
                     PreCourseName = c.Prerequisites.Any()
                         ? string.Join("، ", c.Prerequisites.Select(p => p.PrerequisiteCourse.Name))
                         : "لا يوجد مقرر مطلوب لهذا المقرر",
                     ProfessorName = c.Classes
                         .Select(cl => cl.Professor.FullName)
                         .FirstOrDefault() ?? "لا يوجد دكتور لهذا المقرر",
                     DepartmentName = c.Department != null ? c.Department.Name : "لا يوجد قسم لهذا المقرر",
                     DivisionNames = c.CourseDivisions
                      .Where(cd => cd.Division != null)
                      .Select(cd => cd.Division.Name)
                      .Distinct()
                      .ToList()
                 });
            int totalCount = await query.CountAsync();

            var pagedData = await query
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return pagedData;

        }


        //    public async Task<IEnumerable<CourseDto>> GetCoursesBySemesterWithPreCourseNameAsync(byte semester)
        //    {
        //        return await _dbContext.Courses
        //            .AsNoTrackingWithIdentityResolution()
        //            .Where(c => c.Semester == semester)
        //            .Select(c => new CourseDto
        //            {
        //                Id = c.Id,
        //                Name = c.Name,
        //                Description = c.Description,
        //                Credits = c.Credits,
        //                Status = c.Status,
        //                Code = c.Code,
        //                CurrentEnrolledStudents = _dbContext.Enrollments.Count(e => e.CourseId == c.Id),
        //                MaxSeats = c.MaxSeats,
        //                Semester = c.Semester,
        //                PreCourseName = c.PreCourse != null ? c.PreCourse.Name : "لا يوجد مقرر مطلوب لهذا المقرر",
        //                ProfessorName = c.Classes.Select(c => c.Professor.FullName).FirstOrDefault() ?? "لا يوجد دكتور لهذا المقرر",
        //                DepartmentName = c.CourseDepartments.Select(d => d.Department.Name).FirstOrDefault() ?? "لا يوجد قسم لهذا المقرر"
        //            })
        //            .ToListAsync();
        //    }

        //    public async Task<IEnumerable<CourseDto>> GetCoursesByDepartmentIdWithPreCourseNameAsync(int departmentId)
        //    {
        //        return await _dbContext.CourseDepartments
        //            .AsNoTrackingWithIdentityResolution()
        //            .Where(c => c.DepartmentId == departmentId)
        //            .Select(c => new CourseDto
        //            {
        //                Id = c.Id,
        //                Name = c.Course.Name,
        //                Description = c.Course.Description,
        //                Credits = c.Course.Credits,
        //                Status = c.Course.Status,
        //                Code = c.Course.Code,
        //                Semester = c.Course.Semester,
        //                CurrentEnrolledStudents = _dbContext.Enrollments.Count(e => e.CourseId == c.Course.Id),
        //                MaxSeats = c.Course.MaxSeats,
        //                PreCourseName = c.Course.PreCourse != null ? c.Course.Name : "لا يوجد مقرر مطلوب لهذا المقرر",
        //                DepartmentName = c.Department.Name,
        //                ProfessorName = c.Course.Classes.Select(c => c.Professor.FullName).FirstOrDefault() ?? "لا يوجد دكتور لهذا المقرر"
        //            })
        //            .ToListAsync();
        //    }
        public async Task<IEnumerable<CourseDto>> GetCoursesByProfessorIdWithPreCourseNameAsync(int professorId)
        {
            var courses = await _dbContext.Classes
                .Include(c => c.Course)
                    .ThenInclude(c => c.Prerequisites)
                .Include(c => c.Professor)
                .Include(c => c.Course.CourseDivisions)
                    .ThenInclude(cd => cd.Division)
                        .ThenInclude(d => d.Department)
                .Where(c => c.ProfessorId == professorId)
                .Select(c => new CourseDto
                {
                    Id = c.CourseId,
                    Name = c.Course.Name,
                    Description = c.Course.Description,
                    Credits = c.Course.Credits,
                    Status = c.Course.Status,
                    Semester = c.Course.Semester,
                    Code = c.Course.Code,
                    MaxSeats = c.Course.MaxSeats,
                    ProfessorName = c.Professor.FullName,

                    PreCourseName = c.Course.Prerequisites
                                        .Select(p => p.PrerequisiteCourse.Name)
                                        .FirstOrDefault() ?? "لا يوجد مقرر مطلوب لهذا المقرر",

                    DepartmentName = c.Course.CourseDivisions
                                        .Select(cd => cd.Division.Department.Name)
                                        .FirstOrDefault() ?? "لا يوجد قسم لهذا المقرر",
                    DivisionNames = c.Course.CourseDivisions
                     .Where(cd => cd.Division != null)
                     .Select(cd => cd.Division.Name)
                     .Distinct()
                     .ToList()


                })
                .Distinct()
                .ToListAsync();

            

            return courses;
        }



        public async Task<bool> CourseExistsAsync(string courseName)
        {
            return await _dbContext.Courses
                 .AnyAsync(d => d.Name == courseName);
        }

        public async Task<CourseDto> GetByIdWithPreCourseNameAsync(int id)
        {
            var courseDto = await _dbContext.Courses
                .AsNoTrackingWithIdentityResolution()
                .Include(c => c.Prerequisites)
                .Include(c => c.Classes)
                    .ThenInclude(cls => cls.Professor)
                .Include(c => c.CourseDivisions)
                    .ThenInclude(cd => cd.Division)
                        .ThenInclude(d => d.Department)
                .Where(c => c.Id == id)
                .Select(c => new CourseDto
                {
                    Id = c.Id,
                    Name = c.Name,
                    Description = c.Description,
                    Credits = c.Credits,
                    Status = c.Status,
                    Code = c.Code,
                    CurrentEnrolledStudents = _dbContext.Enrollments.Count(e => e.CourseId == c.Id),
                    MaxSeats = c.MaxSeats,
                    Semester = c.Semester,
                    PreCourseName = c.Prerequisites.Any()
                        ? string.Join("، ", c.Prerequisites.Select(p => p.PrerequisiteCourse.Name))
                        : "لا يوجد مقرر مطلوب لهذا المقرر",
                    ProfessorName = c.Classes.Select(cls => cls.Professor.FullName).FirstOrDefault() ?? "لا يوجد دكتور لهذا المقرر",
                    DepartmentName = c.CourseDivisions
                                      .Select(cd => cd.Division.Department.Name)
                                      .FirstOrDefault() ?? "لا يوجد قسم لهذا المقرر",
                    DivisionNames = c.CourseDivisions
                     .Where(cd => cd.Division != null)
                     .Select(cd => cd.Division.Name)
                     .Distinct()
                     .ToList()
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
        public async Task<Department> GetDepartmentByNameAsync(string name)
        {
            return await _dbContext.Departments.FirstOrDefaultAsync(d => d.Name == name);
        }

        public async Task<List<Division>> GetDivisionsByNamesAsync(List<string> names)
        {
            return await _dbContext.Divisions.Where(d => names.Contains(d.Name)).ToListAsync();
        }

        public async Task<Course> GetCoursesByNamesAsync(string names)
        {
            return await _dbContext.Courses.Where(c => names.Contains(c.Name)).FirstOrDefaultAsync();
        }
         public async Task<List<Course>> GetCoursesByNamesPreAsync(List<string> names)
        {
            return await _dbContext.Courses.Where(c => names.Contains(c.Name)).ToListAsync();
        }

        public async Task AddCourseAsync(Course course)
        {
            await _dbContext.Courses.AddAsync(course);
        }

        public async Task SaveChangesAsync()
        {
            await _dbContext.SaveChangesAsync();
        }
        //public async Task<int> CountByStatusAsync()
        //{
        //    return await _dbContext.Courses
        //        .Where(c => c.Status == "نشط")
        //        .CountAsync();
        //}

        //    public async Task<IEnumerable<string>> GetAllPreRequisiteCoursesAsync()
        //    {
        //        return await _dbContext.Courses
        //            .Where(c => c.PreCourseId != null) // Only get courses with prerequisites
        //            .Select(c => c.PreCourse.Name) // Select the name of the prerequisite course
        //            .Distinct() // Remove duplicates
        //            .ToListAsync();
        //    }

        //    public async Task<IEnumerable<CourseRegistrationStatsDto>> GetCourseRegistrationStatsByCourseOverTimeAsync(int courseId)
        //    {
        //        return await _dbContext.Enrollments
        //        .Where(e => e.CourseId == courseId)
        //        .GroupBy(e => new { e.AddedEnrollmentDate.Year, e.AddedEnrollmentDate.Month })
        //        .Select(g => new CourseRegistrationStatsDto
        //        {
        //            CourseId = courseId,
        //            CourseName = g.First().Course.Name,
        //            Year = g.Key.Year,
        //            Month = g.Key.Month,
        //            TotalRegistrations = g.Count(e => e.DeletedEnrollmentDate == null), // التسجيل الساري
        //            TotalCancellations = g.Count(e => e.DeletedEnrollmentDate != null)  // التسجيل الملغى
        //        })
        //        .ToListAsync();
        //    }

        public async Task<List<CourseStudentDto>> GetCoursesByStudentIdAsync(int studentId)
        {
            return await _dbContext.Enrollments
                .Where(e => e.StudentId == studentId)
                .Include(e => e.Course)
                    .ThenInclude(c => c.Prerequisites)
                        .ThenInclude(p => p.PrerequisiteCourse)
                .Include(e => e.Course.Classes)
                    .ThenInclude(cls => cls.Professor)
                .Include(e => e.Course.CourseDivisions)
                    .ThenInclude(cd => cd.Division)
                        .ThenInclude(d => d.Department)
                .Select(e => new CourseStudentDto
                {
                    Id = e.Course.Id,
                    Name = e.Course.Name,
                    Description = e.Course.Description,
                    Credits = e.Course.Credits,
                    Code = e.Course.Code,
                    Semester = e.Course.Semester,
                    
                    ProfessorName = e.Course.Classes
                        .Select(cls => cls.Professor.FullName)
                        .FirstOrDefault() ?? "لا يوجد مدرس لهذا المقرر",
                    PreCourseName = e.Course.Prerequisites.Any()
                        ? string.Join("، ", e.Course.Prerequisites.Select(p => p.PrerequisiteCourse.Name))
                        : "لا يوجد مقرر مطلوب لهذا المقرر",
                    DepartmentName = e.Course.CourseDivisions
                        .Select(cd => cd.Division.Department.Name)
                        .FirstOrDefault() ?? "لا يوجد قسم لهذا المقرر",
                    DivisionNames = string.Join("، ",
                        e.Course.CourseDivisions
                            .Where(cd => cd.Division != null)
                            .Select(cd => cd.Division.Name)
                            .Distinct()),
                    EnrollmentStatus = e.IsCompleted
                })
                
                .ToListAsync();
        }


        //    public async Task<IEnumerable<string>> GetAllCoursesStatusesAsync()
        //    {
        //        var status = await _dbContext.Courses
        //            .Select(d => d.Status)
        //            .Distinct()
        //            .ToListAsync();

        //        return status;
        //    }

        public async Task<IEnumerable<string>> GetAllCoursesNameAsync()
        {
            var names = await _dbContext.Courses
           .Select(d => d.Name)
           .ToListAsync();

            return names;
        }

        public async Task<IEnumerable<FilterCourseDto>> GetFilteredCoursesAsync(string? courseName, string? departmentName, string? courseStatus, string? divisionName)
        {
            var query = _dbContext.Courses
                .Include(c => c.Prerequisites)
                .Include(c => c.CourseDivisions)
                    .ThenInclude(cd => cd.Division)
                        .ThenInclude(d => d.Department)
                .AsNoTracking()
                .Select(c => new FilterCourseDto
                {
                    Id = c.Id,
                    Name = c.Name,
                    Description = c.Description,
                    Credits = c.Credits,
                    Status = c.Status,
                    Code = c.Code,
                    Semester = c.Semester,
                    MaxSeats = c.MaxSeats,
                    CurrentEnrolledStudents = _dbContext.Enrollments.Count(e => e.CourseId == c.Id),
                    PreCourseName = c.Prerequisites.Any()
                        ? string.Join("، ", c.Prerequisites.Select(p => p.PrerequisiteCourse.Name))
                        : "لا يوجد مقرر مطلوب لهذا المقرر",
                    DepartmentName = c.CourseDivisions
                                      .Select(cd => cd.Division.Department.Name)
                                      .Distinct()
                                      .FirstOrDefault() ?? "لا يوجد قسم لهذا المقرر",
                    DivisionNames = c.CourseDivisions
                     .Where(cd => cd.Division != null)
                     .Select(cd => cd.Division.Name)
                     .Distinct()
                     .ToList()
                });

            // فلترة حسب الحالة
            if (!string.IsNullOrWhiteSpace(courseStatus))
            {
                query = query.Where(c => c.Status.Contains(courseStatus));
            }

            var courses = await query.ToListAsync();

            // فلترة حسب اسم الكورس
            if (!string.IsNullOrWhiteSpace(courseName))
            {
                courseName = NormalizeArabicText(courseName);
                courses = courses
                    .Where(c => NormalizeArabicText(c.Name).Contains(courseName))
                    .ToList();
            }

            // فلترة حسب القسم
            if (!string.IsNullOrWhiteSpace(departmentName))
            {
                departmentName = NormalizeArabicText(departmentName);
                courses = courses
                    .Where(c => !string.IsNullOrEmpty(c.DepartmentName)
                                && NormalizeArabicText(c.DepartmentName).Contains(departmentName))
                    .ToList();
            }
            if (!string.IsNullOrWhiteSpace(divisionName))
            {
                divisionName = NormalizeArabicText(divisionName);
                courses = courses
                    .Where(c => c.DivisionNames
                                .Any(d => NormalizeArabicText(d).Contains(divisionName)))
                    .ToList();
            }



            return courses;
        }



        private string NormalizeArabicText(string text)
        {
            return text.Replace('أ', 'ا')
                       .Replace('إ', 'ا')
                       .Replace('آ', 'ا')
                       .Replace('ى', 'ي')
                       .Replace('ه', 'ة');
        }
        public async Task<CourseStatisticsDto> GetCourseStatisticsAsync(int courseId)
        {
            var enrollments = await _dbContext.Enrollments
                .AsNoTracking()
                .Where(e => e.CourseId == courseId)
                .ToListAsync();

            int enrolledStudentsCount = enrollments.Count;

            // احسب عدد الأقسام عبر CourseDivisions -> Division -> Department
            int departmentsCount = await _dbContext.CourseDivisions
                .AsNoTracking()
                .Where(cd => cd.CourseId == courseId)
                .Select(cd => cd.Division.DepartmentId)
                .Distinct()
                .CountAsync();

            decimal averageGrade = 0;
            if (enrollments.Any())
            {
                averageGrade = enrollments
                    .Where(e => e.FinalGrade.HasValue)
                    .Select(e => e.FinalGrade.Value)
                    .DefaultIfEmpty(0)
                    .Average();
            }

            decimal successRate = 0;
            int totalStudentsWithGrades = enrollments.Count(e => e.FinalGrade.HasValue);
            if (totalStudentsWithGrades > 0)
            {
                int passedStudents = enrollments.Count(e =>
                    e.FinalGrade.HasValue && e.FinalGrade.Value >= PASSING_GRADE);
                successRate = (decimal)passedStudents / totalStudentsWithGrades * 100;
            }

            var statistics = new CourseStatisticsDto
            {
                CourseId = courseId,
                EnrolledStudentsCount = enrolledStudentsCount,
                DepartmentsCount = departmentsCount,
                AverageGrade = Math.Round(averageGrade, 2),
                SuccessRate = Math.Round(successRate, 2)
            };

            return statistics;
        }


        //    public async Task<int> CountActiveCourseAsync()
        //    {
        //        return await _dbContext.Courses
        //             .Where(e => e.Status == "نشط")
        //             .CountAsync();
        //    }
        //    public async Task<IEnumerable<CourseDto>> SearchCoursesWithCourseNameAndStatusAsync(string searchTerm, string status)
        //    {
        //        return await _dbContext.Classes
        //             .AsNoTrackingWithIdentityResolution()
        //             .Where(c => c.Course.Name.Contains(searchTerm) && c.Course.Status == status)
        //             .Select(c => new CourseDto
        //             {
        //                 Id = c.CourseId,
        //                 Name = c.Course.Name,
        //                 Description = c.Course.Description,
        //                 Credits = c.Course.Credits,
        //                 Status = c.Course.Status,
        //                 Semester = c.Course.Semester,
        //                 Code = c.Course.Code,
        //                 CurrentEnrolledStudents = _dbContext.Enrollments.Count(e => e.CourseId == c.Course.Id),
        //                 MaxSeats = c.Course.MaxSeats,
        //                 PreCourseName = c.Course.PreCourse != null ? c.Course.PreCourse.Name : "لا يوجد مقرر مطلوب لهذا المقرر",
        //                 ProfessorName = c.Professor.FullName,
        //                 DepartmentName = c.Course.CourseDepartments.Select(d => d.Department.Name).FirstOrDefault() ?? "لا يوجد قسم لهذا المقرر"
        //             })
        //             .ToListAsync();
        //    }
        public async Task<Course> GetByIdWithEnrollmentsAsync(int id)
        {
            return await _dbContext.Courses
                .Include(c => c.Enrollments)
                .FirstOrDefaultAsync(c => c.Id == id);
        }
    }
}
