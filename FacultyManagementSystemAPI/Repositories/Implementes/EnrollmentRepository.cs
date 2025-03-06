using FacultyManagementSystemAPI.Data;
using FacultyManagementSystemAPI.Models.DTOs.Enrollment;
using FacultyManagementSystemAPI.Models.Entities;
using FacultyManagementSystemAPI.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace FacultyManagementSystemAPI.Repositories.Implementes
{
    public class EnrollmentRepository(AppDbContext dbContext) : GenericRepository<Enrollment>(dbContext), IEnrollmentRepository
    {
        private readonly AppDbContext _dbContext = dbContext;

        public async Task<IEnumerable<EnrollmentDto>> GetAllIncludeStudentNameCourseNameAsync()
        {
            return await _dbContext.Enrollments
                 .AsNoTrackingWithIdentityResolution()
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
                    CourseId = e.CourseId,
                    StudentName = _dbContext.Students
                     .AsNoTrackingWithIdentityResolution()
                        .Where(s => s.Id == e.StudentId)
                        .Select(s => s.Name)
                        .FirstOrDefault(),
                    CourseName = _dbContext.Courses
                     .AsNoTrackingWithIdentityResolution()
                        .Where(c => c.Id == e.CourseId)
                        .Select(c => c.Name)
                        .FirstOrDefault()
                })
                .ToListAsync();
        }

        public async Task<EnrollmentDto> GetByIdIncludeStudentNameCourseNameAsync(int id)
        {
            var enrollmentDto = await _dbContext.Enrollments
                 .AsNoTrackingWithIdentityResolution()
                .Where(e => e.Id == id)
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
                    CourseId = e.CourseId,
                    StudentName = _dbContext.Students
                        .Where(s => s.Id == e.StudentId)
                         .AsNoTrackingWithIdentityResolution()
                        .Select(s => s.Name)
                        .FirstOrDefault(),
                    CourseName = _dbContext.Courses
                        .Where(c => c.Id == e.CourseId)
                         .AsNoTrackingWithIdentityResolution()
                        .Select(c => c.Name)
                        .FirstOrDefault()
                })
                .FirstOrDefaultAsync();

            return enrollmentDto;
        }

        public async Task<IEnumerable<EnrollmentDto>> GetBySemesterAsync(string name)
        {
            var enrollments = await _dbContext.Enrollments
                .Where(e => e.Semester.Contains(name))
                .AsNoTrackingWithIdentityResolution()
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

            return enrollments;
        }

        public async Task<IEnumerable<EnrollmentDto>> GetByStudentIdAsync(int studentId)
        {
            return await _dbContext.Enrollments
                .AsNoTrackingWithIdentityResolution()
                .Where(e => e.StudentId == studentId)
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
                    CourseId = e.CourseId,
                    StudentName = _dbContext.Students
                        .Where(s => s.Id == e.StudentId)
                         .AsNoTrackingWithIdentityResolution()
                        .Select(s => s.Name)
                        .FirstOrDefault(),
                    CourseName = _dbContext.Courses
                        .Where(c => c.Id == e.CourseId)
                         .AsNoTrackingWithIdentityResolution()
                        .Select(c => c.Name)
                        .FirstOrDefault()
                })
                .ToListAsync();
        }

        public async Task<IEnumerable<EnrollmentDto>> GetByCourseIdAsync(int courseId)
        {
            return await _dbContext.Enrollments
                .AsNoTrackingWithIdentityResolution()
                .Where(e => e.CourseId == courseId)
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
                    StudentName = _dbContext.Students
                        .Where(s => s.Id == e.StudentId)
                        .AsNoTrackingWithIdentityResolution()
                        .Select(s => s.Name)
                        .FirstOrDefault(),
                    CourseId = e.CourseId,
                    CourseName = _dbContext.Courses
                        .Where(c => c.Id == e.CourseId)
                         .AsNoTrackingWithIdentityResolution()
                        .Select(c => c.Name)
                        .FirstOrDefault()
                })
                .ToListAsync();
        }

        public async Task<int> CountAsync()
        {
            return await _dbContext.Enrollments.CountAsync();
        }

        public async Task<int> CountDeletedAsync()
        {
            return await _dbContext.Enrollments.CountAsync(e => e.DeletedEnrollmentDate != null);
        }
     
        public async Task<Enrollment> GetByStudentAndCourseIdAsync(int studentId, int courseId)
        {
            return await _dbContext.Enrollments
                .FirstOrDefaultAsync(e => e.StudentId == studentId && e.CourseId == courseId);
        }

        public async Task UpdateAsync(Enrollment enrollment)
        {
            _dbContext.Enrollments.Update(enrollment);
            await _dbContext.SaveChangesAsync();
        }
    }
}