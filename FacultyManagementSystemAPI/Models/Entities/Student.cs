using FacultyManagementSystemAPI.Data;
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

        public decimal GPA_Average
        {
            get
            {
                var gpas = new List<decimal?> { GPA1, GPA2, GPA3, GPA4, GPA5, GPA6, GPA7, GPA8 }
                            .Where(gpa => gpa.HasValue)
                            .Select(gpa => gpa.Value)
                            .ToList();

                return gpas.Any() ? gpas.Average() : 0;
            }
        }

        [Column(TypeName = "decimal(10,2)")]
        public decimal? GPA1 { get; set; }

        [Column(TypeName = "decimal(10,2)")]
        public decimal? GPA2 { get; set; }

        [Column(TypeName = "decimal(10,2)")]
        public decimal? GPA3 { get; set; }

        [Column(TypeName = "decimal(10,2)")]
        public decimal? GPA4 { get; set; }

        [Column(TypeName = "decimal(10,2)")]
        public decimal? GPA5 { get; set; }

        [Column(TypeName = "decimal(10,2)")]
        public decimal? GPA6 { get; set; }

        [Column(TypeName = "decimal(10,2)")]
        public decimal? GPA7 { get; set; }

        [Column(TypeName = "decimal(10,2)")]
        public decimal? GPA8 { get; set; }

        [Required]
        [Column(TypeName = "decimal(10,2)")]
        public decimal High_School_degree { get; set; }

        [Required]
        [MaxLength(50)]
        public string High_School_Section { get; set; }

        [Required]
        public int CreditsCompleted { get; set; }

        [Required]
        public string StudentLevel { get; set; }

        [Required]
        public string status { get; set; }

        [Required]
        public string ImagePath { get; set; }

        public int DivisionId { get; set; }
        [ForeignKey(nameof(DivisionId))]
        public Division Division { get; set; }

        public List<Enrollment> Enrollments { get; set; } = new List<Enrollment>();
        public List<Attendance> Attendances { get; set; } = new List<Attendance>();
        public virtual ICollection<AcademicWarning> AcademicWarnings { get; set; } = new List<AcademicWarning>();


        public string ApplicationUserId { get; set; }
        [ForeignKey(nameof(ApplicationUserId))]
        public ApplicationUser ApplicationUser { get; set; }
    }
}
