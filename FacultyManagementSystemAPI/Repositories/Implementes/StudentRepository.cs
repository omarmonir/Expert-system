using FacultyManagementSystemAPI.Data;
using FacultyManagementSystemAPI.Models.DTOs.Enrollment;
using FacultyManagementSystemAPI.Models.DTOs.Student;
using FacultyManagementSystemAPI.Models.Entities;
using FacultyManagementSystemAPI.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;
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
                     DateOfBirth = s.DateOfBirth,
                     EnrollmentDate = s.EnrollmentDate,
                     Phone = s.Phone,
                     Gender = s.Gender,
                     NationalId = s.NationalId,
                     Nationality = s.Nationality,
                     Semester = s.Semester,
                     GPA_Average = s.GPA_Average,
                     status = s.status,
                     StudentLevel = s.StudentLevel,
                     High_School_degree = s.High_School_degree,
                     High_School_Section = s.High_School_Section,
                     CreditsCompleted = s.CreditsCompleted,
                     ImagePath = s.ImagePath,
                     DepartmentName = s.Department.Name
                 }).ToListAsync();
        }

        public async Task<StudentDto> GetByIdWithDepartmentNameAsync(int id)
        {

            var studentDto = await _dbContext.Students
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
                     DateOfBirth = s.DateOfBirth,
                     NationalId = s.NationalId,
                     Nationality = s.Nationality,
                     Semester = s.Semester,
                     GPA_Average = s.GPA_Average,
                     status = s.status,
                     StudentLevel = s.StudentLevel,
                     High_School_degree = s.High_School_degree,
                     High_School_Section = s.High_School_Section,
                     CreditsCompleted = s.CreditsCompleted,
                     ImagePath = s.ImagePath,
                     DepartmentName = s.Department.Name
                 }).FirstOrDefaultAsync();

            return studentDto;
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
                     DateOfBirth = s.DateOfBirth,
                     NationalId = s.NationalId,
                     Nationality = s.Nationality,
                     Semester = s.Semester,
                     GPA_Average = s.GPA_Average,
                     status = s.status,
                     StudentLevel = s.StudentLevel,
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
            var gradesOfStudents = await _dbContext.Enrollments
                 .AsNoTrackingWithIdentityResolution()
                 .Where(s => s.StudentId == id)
                 .Select(s => new StudentWithGradesDto
                 {
                     Id = s.Student.Id,
                     Name = s.Student.Name,
                     Exam1Grade = s.Exam1Grade,
                     Exam2Grade = s.Exam2Grade,
                     FinalGrade = s.FinalGrade,
                     Grade = s.Grade
                 }).FirstOrDefaultAsync();
            return gradesOfStudents;
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

        public async Task<IEnumerable<StudentDto>> GetUnenrolledStudentsAsync()
        {
            return await _dbContext.Students
                .Where(s => !s.Enrollments.Any()) // الطلاب غير المسجلين في أي كورس
                .Select(s => new StudentDto
                {
                    Id = s.Id,
                    Name = s.Name,
                    NationalId = s.NationalId,
                    Gender = s.Gender,
                    DateOfBirth = s.DateOfBirth,
                    Address = s.Address,
                    Nationality = s.Nationality,
                    Email = s.Email,
                    Phone = s.Phone,
                    Semester = s.Semester,
                    EnrollmentDate = s.EnrollmentDate,
                    GPA_Average = s.GPA_Average,
                    status = s.status,
                    StudentLevel = s.StudentLevel,
                    High_School_degree = s.High_School_degree,
                    High_School_Section = s.High_School_Section,
                    CreditsCompleted = s.CreditsCompleted,
                    DepartmentName = s.Department.Name,
                    ImagePath = s.ImagePath
                }).ToListAsync();
        }

        public async Task<IEnumerable<StudentDto>> GetUnenrolledStudentsByDepartmentAsync(int departmentId)
        {
            return await _dbContext.Students
                .Where(s => !s.Enrollments.Any() && s.DepartmentId == departmentId) // الطلاب غير المسجلين في أي كورس داخل القسم المحدد
                .Select(s => new StudentDto
                {
                    Id = s.Id,
                    Name = s.Name,
                    NationalId = s.NationalId,
                    Gender = s.Gender,
                    DateOfBirth = s.DateOfBirth,
                    Address = s.Address,
                    Nationality = s.Nationality,
                    Email = s.Email,
                    Phone = s.Phone,
                    Semester = s.Semester,
                    EnrollmentDate = s.EnrollmentDate,
                    GPA_Average = s.GPA_Average,
                    status = s.status,
                    StudentLevel = s.StudentLevel,
                    High_School_degree = s.High_School_degree,
                    High_School_Section = s.High_School_Section,
                    CreditsCompleted = s.CreditsCompleted,
                    DepartmentName = s.Department.Name,
                    ImagePath = s.ImagePath
                }).ToListAsync();
        }

        public async Task<IEnumerable<StudentDto>> GetUnenrolledStudentsBySemesterAsync(byte semester)
        {
            return await _dbContext.Students
                .Where(s => !s.Enrollments.Any() && s.Semester == semester) // الطلاب غير المسجلين في أي كورس بالفصل الدراسي المحدد
                .Select(s => new StudentDto
                {
                    Id = s.Id,
                    Name = s.Name,
                    NationalId = s.NationalId,
                    Gender = s.Gender,
                    DateOfBirth = s.DateOfBirth,
                    Address = s.Address,
                    Nationality = s.Nationality,
                    Email = s.Email,
                    Phone = s.Phone,
                    Semester = s.Semester,
                    EnrollmentDate = s.EnrollmentDate,
                    GPA_Average = s.GPA_Average,
                    status = s.status,
                    StudentLevel = s.StudentLevel,
                    High_School_degree = s.High_School_degree,
                    High_School_Section = s.High_School_Section,
                    CreditsCompleted = s.CreditsCompleted,
                    DepartmentName = s.Department.Name,
                    ImagePath = s.ImagePath
                }).ToListAsync();
        }

        public async Task<IEnumerable<EnrollmentDto>> GetEnrollmentsByDateRangeAsync(DateTime? minDate, DateTime? maxDate)
        {
            var query = _dbContext.Enrollments
                .Include(e => e.Student)
                .Include(e => e.Course)
                .AsQueryable();

            if (minDate.HasValue)
                query = query.Where(e => e.AddedEnrollmentDate >= minDate.Value);

            if (maxDate.HasValue)
                query = query.Where(e => e.AddedEnrollmentDate <= maxDate.Value);

            return await query
                .Select(e => new EnrollmentDto
                {
                    Id = e.Id,
                    Semester = e.Semester,
                    AddedEnrollmentDate = e.AddedEnrollmentDate,
                    DeletedEnrollmentDate = e.DeletedEnrollmentDate,
                    FinalGrade = e.FinalGrade,
                    Exam1Grade = e.Exam1Grade,
                    Exam2Grade = e.Exam2Grade,
                    Grade = e.Grade,
                    StudentId = e.StudentId,
                    StudentName = e.Student.Name,
                    CourseId = e.CourseId,
                    CourseName = e.Course.Name
                })
                .ToListAsync();
        }

        public async Task<IEnumerable<StudentDto>> GetFilteredStudentsAsync(StudentFilterDto filter)
        {
            var query = _dbContext.Students.AsNoTrackingWithIdentityResolution().AsQueryable();

            if (!string.IsNullOrWhiteSpace(filter.Name))
                query = query.Where(s => s.Name.Contains(filter.Name));

            if (!string.IsNullOrWhiteSpace(filter.NationalId))
                query = query.Where(s => s.NationalId.Contains(filter.NationalId));

            if (!string.IsNullOrWhiteSpace(filter.Email))
                query = query.Where(s => s.Email.Contains(filter.Email));

            if (!string.IsNullOrWhiteSpace(filter.Gender))
                query = query.Where(s => s.Gender == filter.Gender);

            if (!string.IsNullOrWhiteSpace(filter.Phone))
                query = query.Where(s => s.Phone.Contains(filter.Phone));

            if (filter.Semester.HasValue)
                query = query.Where(s => s.Semester == filter.Semester.Value);

            if (filter.MinHigh_School_degree.HasValue)
                query = query.Where(s => s.High_School_degree >= filter.MinHigh_School_degree.Value);

            if (filter.MaxHigh_School_degree.HasValue)
                query = query.Where(s => s.High_School_degree <= filter.MaxHigh_School_degree.Value);

            if (!string.IsNullOrWhiteSpace(filter.High_School_Section))
                query = query.Where(s => s.High_School_Section == filter.High_School_Section);

            if (filter.MinEnrollmentDate.HasValue)
                query = query.Where(s => s.EnrollmentDate >= filter.MinEnrollmentDate.Value);

            if (filter.MaxEnrollmentDate.HasValue)
                query = query.Where(s => s.EnrollmentDate <= filter.MaxEnrollmentDate.Value);

            if (filter.MinDateOfBirth.HasValue)
                query = query.Where(s => s.DateOfBirth >= filter.MinDateOfBirth.Value);

            if (filter.MaxDateOfBirth.HasValue)
                query = query.Where(s => s.DateOfBirth <= filter.MaxDateOfBirth.Value);

            if (filter.MinGPA.HasValue)
                query = query.Where(s => s.GPA_Average >= filter.MinGPA.Value);

            if (filter.MaxGPA.HasValue)
                query = query.Where(s => s.GPA_Average <= filter.MaxGPA.Value);

            if (filter.DepartmentId.HasValue)
                query = query.Where(s => s.DepartmentId == filter.DepartmentId.Value);

            // Sorting
            if (!string.IsNullOrWhiteSpace(filter.SortBy))
            {
                var isDescending = filter.SortOrder?.ToUpper() == "DESC";

                query = filter.SortBy.ToLower() switch
                {
                    "name" => isDescending ? query.OrderByDescending(s => s.Name) : query.OrderBy(s => s.Name),
                    "nationalId" => isDescending ? query.OrderByDescending(s => s.NationalId) : query.OrderBy(s => s.NationalId),
                    "email" => isDescending ? query.OrderByDescending(s => s.Email) : query.OrderBy(s => s.Email),
                    "gpa" => isDescending ? query.OrderByDescending(s => s.GPA_Average) : query.OrderBy(s => s.GPA_Average),
                    "semester" => isDescending ? query.OrderByDescending(s => s.Semester) : query.OrderBy(s => s.Semester),
                    "high_school_degree" => isDescending ? query.OrderByDescending(s => s.High_School_degree) : query.OrderBy(s => s.High_School_degree),
                    "enrollmentDate" => isDescending ? query.OrderByDescending(s => s.EnrollmentDate) : query.OrderBy(s => s.EnrollmentDate),
                    "dateOfBirth" => isDescending ? query.OrderByDescending(s => s.DateOfBirth) : query.OrderBy(s => s.DateOfBirth),
                    _ => query.OrderBy(s => s.Id) // افتراضيًا الترتيب حسب ID
                };
            }

            var students = await query
                .Select(s => new StudentDto
                {
                    Id = s.Id,
                    Name = s.Name,
                    NationalId = s.NationalId,
                    Gender = s.Gender,
                    Address = s.Address,
                    Nationality = s.Nationality,
                    Email = s.Email,
                    Phone = s.Phone,
                    Semester = s.Semester,
                    EnrollmentDate = s.EnrollmentDate,
                    GPA_Average = s.GPA_Average,
                    status = s.status,
                    StudentLevel = s.StudentLevel,
                    High_School_degree = s.High_School_degree,
                    High_School_Section = s.High_School_Section,
                    CreditsCompleted = s.CreditsCompleted,
                    ImagePath = s.ImagePath,
                    DepartmentName = s.Department.Name
                })
                .ToListAsync();

            return students;
        }

        public async Task<IEnumerable<StudentDto>> GetStudentsByDepartmentAndNameAsync(string? departmentName, string? studentName, string? studentStatus)
        {
            var studentQuery = _dbContext.Students.AsNoTrackingWithIdentityResolution();

            // فلترة حسب حالة الطالب (إذا كانت محددة)
            if (!string.IsNullOrWhiteSpace(studentStatus))
            {
                studentQuery = studentQuery.Where(s => s.status == studentStatus);
            }

            // جلب البيانات أولًا إلى الذاكرة ثم تطبيق الفلترة
            var students = await studentQuery
                .Select(s => new StudentDto
                {
                    Id = s.Id,
                    Name = s.Name,
                    NationalId = s.NationalId,
                    Gender = s.Gender,
                    Address = s.Address,
                    Nationality = s.Nationality,
                    Email = s.Email,
                    Phone = s.Phone,
                    Semester = s.Semester,
                    DateOfBirth = s.DateOfBirth,
                    EnrollmentDate = s.EnrollmentDate,
                    GPA_Average = s.GPA_Average,
                    status = s.status,
                    StudentLevel = s.StudentLevel,
                    High_School_degree = s.High_School_degree,
                    High_School_Section = s.High_School_Section,
                    CreditsCompleted = s.CreditsCompleted,
                    ImagePath = s.ImagePath,
                    DepartmentName = _dbContext.Departments
                        .Where(d => d.Id == s.DepartmentId)
                        .Select(d => d.Name)
                        .FirstOrDefault()
                })
                .ToListAsync(); // تحويل الاستعلام إلى List وإحضاره إلى الذاكرة

            // تطبيق الفلترة بعد جلب البيانات إلى الذاكرة
            if (!string.IsNullOrWhiteSpace(departmentName))
            {
                departmentName = NormalizeArabicText(departmentName);
                students = students.Where(s => NormalizeArabicText(s.DepartmentName).Contains(departmentName)).ToList();
            }

            if (!string.IsNullOrWhiteSpace(studentName))
            {
                studentName = NormalizeArabicText(studentName);
                students = students.Where(s => NormalizeArabicText(s.Name).Contains(studentName)).ToList();
            }

            return students;
        }

        // دالة استبدال الأحرف
        private string NormalizeArabicText(string? text)
        {
            if (string.IsNullOrWhiteSpace(text))
                return string.Empty;

            return text.Replace('أ', 'ا')
                       .Replace('إ', 'ا')
                       .Replace('آ', 'ا')
                       .Replace('ى', 'ي')
                       .Replace('ه', 'ة');
        }




        public async Task<IEnumerable<StudentDto>> GetAllByDepartmentIdAsync(int departmentId)
        {
            var students = await _dbContext.Students
                .AsNoTrackingWithIdentityResolution()
                .Where(s => s.DepartmentId == departmentId)
                .Select(s => new StudentDto
                {
                    Id = s.Id,
                    Name = s.Name,
                    NationalId = s.NationalId,
                    Gender = s.Gender,
                    DateOfBirth = s.DateOfBirth,
                    Address = s.Address,
                    Nationality = s.Nationality,
                    Email = s.Email,
                    Phone = s.Phone,
                    Semester = s.Semester,
                    EnrollmentDate = s.EnrollmentDate,
                    GPA_Average = s.GPA_Average,
                    status = s.status,
                    StudentLevel = s.StudentLevel,
                    High_School_degree = s.High_School_degree,
                    High_School_Section = s.High_School_Section,
                    CreditsCompleted = s.CreditsCompleted,
                    ImagePath = s.ImagePath,
                    DepartmentName = s.Department.Name
                })
                .ToListAsync();

            return students;
        }

        public async Task<IEnumerable<Student>> GetStudentsByDepartmentNameAsync(string departmentName)
        {
            return await _dbContext.Students
                .Where(s => s.Department.Name == departmentName)
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<int> CountAsync()
        {
            return await _dbContext.Students.CountAsync();
        }

        public async Task<int> CountEnrolledStudentsAsync()
        {
            return await _dbContext.Enrollments
                .Where(e => e.DeletedEnrollmentDate == null)
                .Select(e => e.StudentId)
                .Distinct()
                .CountAsync();
        }

        public async Task<IEnumerable<string>> GetAllStudentStatusesAsync()
        {
            var status = await _dbContext.Students
                .Select(d => d.status)
                .Distinct()
                .ToListAsync();

            return status;
        }

        public async Task<IEnumerable<string>> GetAllStudentLevelsAsync()
        {
            var studentLevel = await _dbContext.Students
                .Select(d => d.StudentLevel)
                .Distinct()
                .ToListAsync();

            return studentLevel;
        }

        public async Task<IEnumerable<string>> GetAllStudentGenderAsync()
        {
            var genders = await _dbContext.Students
                .Select(d => d.Gender)
                .Distinct()
                .ToListAsync();

            return genders;
        }

        public async Task<IEnumerable<StudentExamGradesDto>> GetStudentsWithExamGradesByCourseIdAsync(int courseId)
        {
            return await _dbContext.Enrollments
               .Where(e => e.CourseId == courseId) 
               .Include(e => e.Student) 
               .Select(e => new StudentExamGradesDto
               {
                   StudentId = e.Student.Id,
                   StudentName = e.Student.Name,
                   Exam1Grade = e.Exam1Grade,
                   Exam2Grade = e.Exam2Grade,
                   FinalGrade = e.FinalGrade
               })
               .ToListAsync();
        }
    }


}
