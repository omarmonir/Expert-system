namespace FacultyManagementSystemAPI.Models.DTOs.Courses
{
    public class CourseDto
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public string Description { get; set; }

        public int Credits { get; set; }

        public string Status { get; set; }

        public byte Semester { get; set; }

        public string? PreCourseName { get; set; }
        public string ProfessorName { get; set; }
    }
}
