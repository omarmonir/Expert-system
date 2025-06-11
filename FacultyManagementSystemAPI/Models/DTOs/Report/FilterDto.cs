namespace FacultyManagementSystemAPI.Models.DTOs.Report
{
    public class FilterDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Code { get; set; }
        public string Description { get; set; }
        public int Credits { get; set; }
        public string Status { get; set; }
        public byte Semester { get; set; }
        public int MaxSeats { get; set; }
        public int CurrentEnrolledStudents { get; set; }
        public string PreCourseName { get; set; }
        public string ProfessorName { get; set; }
        public string DepartmentName { get; set; }
        public string DivisionNames { get; set; }

    }
}
