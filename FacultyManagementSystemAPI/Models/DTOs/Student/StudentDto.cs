using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace FacultyManagementSystemAPI.Models.DTOs.Student
{
	public class StudentDto
	{
		public int Id { get; set; }

		public string Name { get; set; }

		public string NationalId { get; set; }

		public string Gender { get; set; }

		public string? Address { get; set; }

		public string? Nationality { get; set; }

		public string Email { get; set; }

		public string Phone { get; set; }

		public byte Semester { get; set; }

		public DateTime EnrollmentDate { get; set; }

		public decimal GPA { get; set; }

		public decimal High_School_degree { get; set; }

		public string High_School_Section { get; set; }

		public int CreditsCompleted { get; set; }

		public string ImagePath { get; set; } 

		public string DepartmentName { get; set; }
	}
}
