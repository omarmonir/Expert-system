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
        [Column(TypeName = "decimal(10,2)")]
        public decimal FinalGrade { get; set; }
        
        [Required]
        [Column(TypeName = "decimal(10,2)")]
        public decimal Exam1Grade { get; set; }
        
        [Required]
        [Column(TypeName = "decimal(10,2)")]
        public decimal Exam2Grade { get; set; }
        
        [Required]
        [Column(TypeName = "decimal(10,2)")]
        public decimal Grade { get; set; }

        // Navigation Properties

        public int StudentId { get; set; }

        [ForeignKey(nameof(StudentId))]
        public Student Student { get; set; }

        public int CourseId { get; set; }

        [ForeignKey(nameof(CourseId))]
        public Course Course { get; set; }
    }

}
