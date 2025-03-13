namespace FacultyManagementSystemAPI.Models.DTOs.Report
{
    public class ProfessorCoursesDto
    {
        public string ProfessorName { get; set; }
        public string CourseNames { get; set; } // ✅ تغيير `List<string>` إلى `string`
    }
}
