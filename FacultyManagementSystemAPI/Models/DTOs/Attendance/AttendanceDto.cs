namespace FacultyManagementSystemAPI.Models.DTOs.Attendance
{
    public class AttendanceDto
    {
        public int Id { get; set; }
        public DateTime Date { get; set; }
        public bool Status { get; set; }
        public int StudentId { get; set; }
        public int ClassesId { get; set; }
        public string StudentName { get; set; }
        public string CourseName { get; set; }
    }
}
