namespace FacultyManagementSystemAPI.Models.DTOs.Courses
{
    public class CourseRegistrationStatsDto
    {
        public int CourseId { get; set; }
        public string CourseName { get; set; }
        public int Year { get; set; }
        public int Month { get; set; }
        public int TotalRegistrations { get; set; }
        public int TotalCancellations { get; set; }
    }
}
