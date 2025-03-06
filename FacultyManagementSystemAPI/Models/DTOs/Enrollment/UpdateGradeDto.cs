namespace FacultyManagementSystemAPI.Models.DTOs.Enrollment
{
    public class UpdateGradeDto
    {
        public int StudentId { get; set; }
        public int CourseId { get; set; }
        public decimal NewGrade { get; set; }
    }
}
