namespace FacultyManagementSystemAPI.Models.DTOs.Report
{
    public class CourseGradeReportDto
    {
        public string StudentName { get; set; }
        public string CourseName { get; set; }
        public decimal Exam1Grade { get; set; }
        public decimal Exam2Grade { get; set; }
        public decimal FinalGrade { get; set; }
    }
}
