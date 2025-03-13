namespace FacultyManagementSystemAPI.Models.DTOs.Report
{
    public class CourseEnrollmentDto
    {
        public string CourseName { get; set; }
        public DateTime EnrollmentDate { get; set; }
        public int EnrollmentCount { get; set; }
    }
}
