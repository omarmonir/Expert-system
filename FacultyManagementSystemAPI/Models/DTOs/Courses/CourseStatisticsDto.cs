namespace FacultyManagementSystemAPI.Models.DTOs.Courses
{
    public class CourseStatisticsDto
    {
        public int CourseId { get; set; }
        public int EnrolledStudentsCount { get; set; }
        public int DepartmentsCount { get; set; }
        public decimal AverageGrade { get; set; }
        public decimal SuccessRate { get; set; }
    }
}
