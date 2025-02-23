using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FacultyManagementSystemAPI.Models.Entities
{
    public class Enrollment
    {
        [Key]
        public int Id { get; set; }

        [MaxLength(50)]
        public string Semester { get; set; }


        [Required]
        public int NumberOFSemster { get; set; }

        [Column(TypeName = "decimal(10,2)")]
        public decimal? FinalGrade
        {
            get
            {
                return (Exam1Grade ?? 0) + (Exam2Grade ?? 0) + (Grade ?? 0);
            }
        }


        [Column(TypeName = "decimal(10,2)")]
        public decimal? Exam1Grade { get; set; }


        [Column(TypeName = "decimal(10,2)")]
        public decimal? Exam2Grade { get; set; }


        [Column(TypeName = "decimal(10,2)")]
        public decimal? Grade { get; set; }

        [Required]
        [Column(TypeName = "DATE")]
        public DateTime AddedEnrollmentDate { get; set; }

        [Column(TypeName = "DATE")]
        public DateTime? DeletedEnrollmentDate { get; set; }

        // Navigation Properties

        public int StudentId { get; set; }

        [ForeignKey(nameof(StudentId))]
        public Student Student { get; set; }

        public int CourseId { get; set; }

        [ForeignKey(nameof(CourseId))]
        public Course Course { get; set; }
        [Required]
        public string IsCompleted { get; set; }
    }
}
