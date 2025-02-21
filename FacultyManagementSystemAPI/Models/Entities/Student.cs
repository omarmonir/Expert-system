using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FacultyManagementSystemAPI.Models.Entities
{
    public class Student
    {
        [Key]
        public int Id { get; set; }

        [MaxLength(100)]
        public string Name { get; set; }

        [MaxLength(14)]
        [MinLength(14)]
        public string NationalId { get; set; }

        [MaxLength(10)]
        public string Gender { get; set; }

        [Required]
        [Column(TypeName = "DATE")]
        public DateTime DateOfBirth { get; set; }

        [MaxLength(100)]
        public string? Address { get; set; }

        [MaxLength(50)]
        public string? Nationality { get; set; }

        [MaxLength(100)]
        [EmailAddress]
        public string Email { get; set; }

        [Phone]
        public string Phone { get; set; }

        [Required]
        public byte Semester { get; set; }

        [Required]
        [Column(TypeName = "DATE")]
        public DateTime EnrollmentDate { get; set; }


        [Required]
        [Column(TypeName = "decimal(10,2)")]
        public decimal GPA { get; set; }
        
        [Required]
        [Column(TypeName = "decimal(10,2)")]
        public decimal High_School_degree { get; set; }  

        [Required]
		[MaxLength(50)]
		public string High_School_Section { get; set; }

        [Required]
        public int CreditsCompleted { get; set; }

        [Required]
		public string ImagePath { get; set; }

		// Navigation Properties

		public int DepartmentId { get; set; }
        [ForeignKey(nameof(DepartmentId))]
        public Department Department { get; set; } // العلاقة مع القسم

        // Many-to-Many with Course By Enrollment
        public List<Enrollment> Enrollments { get; set; } = new List<Enrollment>();
        public List<Attendance> Attendances { get; set; } = new List<Attendance>();

	}

}
