namespace FacultyManagementSystemAPI.Models.DTOs.Report
{
    public class StudentAttendanceDto
    {
        public string StudentName { get; set; }
        public string CourseName { get; set; }
        public int TotalClasses { get; set; }
        public int AttendedClasses { get; set; }
        public double AttendancePercentage { get; set; }
    }
}
