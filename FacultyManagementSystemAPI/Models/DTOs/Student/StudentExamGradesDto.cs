namespace FacultyManagementSystemAPI.Models.DTOs.Student
{
    public class StudentExamGradesDto
    {
        public int StudentId { get; set; }
        public string StudentName { get; set; } 
        public string CourseName { get; set; } 
        public string Code { get; set; } 
        public decimal? Exam1Grade { get; set; } 
        public decimal? Exam2Grade { get; set; }
        public decimal? FinalGrade { get; set; } 
        public decimal? Grade { get; set; } 

    }
}

