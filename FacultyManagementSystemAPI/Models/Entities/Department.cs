using System.ComponentModel.DataAnnotations;

namespace FacultyManagementSystemAPI.Models.Entities
{
    public class Department
    {
        [Key]
        public int Id { get; set; }

        [MaxLength(50)]
        public string Name { get; set; }

        [Required]
        public int ProfessorCount { get; set; }

        [MaxLength(100)]
        public string HeadOfDepartment { get; set; }

        // Navigation Properties

        public List<Student> Students { get; set; } = new List<Student>();
		
		public List<Professor> Professors { get; set; } = new List<Professor>();
       
        public List<CourseDepartment> CourseDepartments { get; set; } = new List<CourseDepartment>();
    }
}
