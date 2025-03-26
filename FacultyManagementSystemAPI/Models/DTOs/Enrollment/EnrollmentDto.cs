namespace FacultyManagementSystemAPI.Models.DTOs.Enrollment
{
    public class EnrollmentDto
    {
        public int Id{ get; set; }
        public int StudentID { get; set; }
        public string StudentName { get; set; }
        public string CourseCode { get; set; }
        public string CourseName { get; set; }
        public DateTime EnrollmentDate { get; set; }
        public string EnrollmentStatus { get; set; }
        public string Semester { get; set; }
    }
}
