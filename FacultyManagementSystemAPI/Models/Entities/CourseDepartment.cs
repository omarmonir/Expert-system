using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FacultyManagementSystemAPI.Models.Entities
{
	public class CourseDepartment
	{
		[Key]
		public int Id { get; set; }

		public int CourseId { get; set; }
		[ForeignKey(nameof(CourseId))]
		public Course Course { get; set; }

		public int DepartmentId { get; set; }
		[ForeignKey(nameof(DepartmentId))]
		public Department Department { get; set; }
	}
}
