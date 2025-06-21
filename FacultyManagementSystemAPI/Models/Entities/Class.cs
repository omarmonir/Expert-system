using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FacultyManagementSystemAPI.Models.Entities
{
	public class Class
	{
		[Key]
		public int Id { get; set; }

		[Required]
		[Column(TypeName = "TIME")]
		public TimeSpan StartTime { get; set; }

		[Required]
		[Column(TypeName = "TIME")]
		public TimeSpan EndTime { get; set; }

		[MaxLength(20)]
		public string Day { get; set; }
		
		[MaxLength(100)]
		public string Location { get; set; }

		public int ProfessorId { get; set; }
		[ForeignKey(nameof(ProfessorId))]
		public Professor Professor { get; set; }

		public int CourseId { get; set; }
		[ForeignKey(nameof(CourseId))]
		public Course Course { get; set; }

		public List<Attendance> Attendances { get; set; } = new List<Attendance>();		
	}
}
