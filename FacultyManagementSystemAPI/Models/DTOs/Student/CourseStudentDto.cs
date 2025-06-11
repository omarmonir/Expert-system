namespace FacultyManagementSystemAPI.Models.DTOs.Student
{
    public class CourseStudentDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Code { get; set; }
        public string Description { get; set; }
        public int Credits { get; set; }
        public byte Semester { get; set; }
        public string PreCourseName { get; set; }
        public string ProfessorName { get; set; }
        public string DepartmentName { get; set; }
        public string DivisionNames { get; set; }
        public string EnrollmentStatus { get; set; }

    }
}
