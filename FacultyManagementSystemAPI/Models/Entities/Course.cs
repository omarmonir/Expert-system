using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FacultyManagementSystemAPI.Models.Entities
{
    public class Course
    {
        [Key]
        public int Id { get; set; }

        [MaxLength(50)]
        public string Name { get; set; }

		[MaxLength(250)]
		public string Description { get; set; }

		[Required]
        public int Credits { get; set; }

        [MaxLength(50)]
        public string Status { get; set; }
        
        [Required]
        public byte Semester { get; set; }

        public int? PreCourseId { get; set; }
		[ForeignKey(nameof(PreCourseId))]
		public virtual Course PreCourse { get; set; }

		// Navigation Properties
		public List<Class> Classes { get; set; } = new List<Class>();
		public List<Enrollment> Enrollments { get; set; } = new List<Enrollment>();
		public List<CourseDepartment> CourseDepartments { get; set; } = new List<CourseDepartment>();
	}

}
