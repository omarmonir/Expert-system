namespace FacultyManagementSystemAPI.Models.DTOs.Enrollment
{
    public class EnrollmentDto
    {
        public int Id { get; set; }
        public string Semester { get; set; }
        public DateTime AddedEnrollmentDate { get; set; }
        public DateTime? DeletedEnrollmentDate { get; set; }
        public decimal? FinalGrade { get; set; }
        public decimal? Exam1Grade { get; set; }
        public decimal? Exam2Grade { get; set; }
        public decimal? Grade { get; set; }
        public int StudentId { get; set; }
        public string StudentName { get; set; }
        public int CourseId { get; set; }
        public string CourseName { get; set; }
    }
}
