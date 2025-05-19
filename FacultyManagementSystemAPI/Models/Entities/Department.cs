using System.ComponentModel.DataAnnotations;

namespace FacultyManagementSystemAPI.Models.Entities
{
    public class Department
    {
        [Key]
        public int Id { get; set; }

        [MaxLength(50)]
        public string Name { get; set; }

        // Navigation Properties
        public List<Professor> Professors { get; set; } = new List<Professor>();
        public List<Course> Courses { get; set; } = new List<Course>();
        public List<Division> Divisions { get; set; } = new List<Division>();
    }
}
