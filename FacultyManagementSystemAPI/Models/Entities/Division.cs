using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FacultyManagementSystemAPI.Models.Entities
{
    public class Division
    {
        [Key]
        public int Id { get; set; }

        [MaxLength(50)]
        public string Name { get; set; }

        public int DepartmentId { get; set; }
        [ForeignKey(nameof(DepartmentId))]
        public Department Department { get; set; }

        // Navigation Properties
        public List<Student> Students { get; set; } = new List<Student>();
        public List<CourseDivision> CourseDivisions { get; set; } = new List<CourseDivision>();
    }
}
