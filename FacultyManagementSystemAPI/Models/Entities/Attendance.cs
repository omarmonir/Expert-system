using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FacultyManagementSystemAPI.Models.Entities
{
	public class Attendance
	{
		[Key]
		public int Id { get; set; }

		[Required]
		public DateTime Date { get; set; }

		[Required]
		public bool Status { get; set; }

		public int ClassesId { get; set; }
		[ForeignKey(nameof(ClassesId))]
		public Class Class { get; set; }

		public int StudentId { get; set; }
		[ForeignKey(nameof(StudentId))]
		public Student Student { get; set; }
	}
}
