namespace FacultyManagementSystemAPI.Models.DTOs.Enrollment
{
    public class UpdateExam2GradeDto
    {
        public int StudentId { get; set; } // تم تغيير EnrollmentId إلى StudentId
        public decimal NewExam2Grade { get; set; }
    }
}
