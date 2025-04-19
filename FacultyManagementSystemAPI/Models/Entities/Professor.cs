using FacultyManagementSystemAPI.Data;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FacultyManagementSystemAPI.Models.Entities
{
    public class Professor
    {
        [Key]
        public int Id { get; set; }

        [MaxLength(100)]
        public string FullName { get; set; }

        [MaxLength(14)]
        [MinLength(14)]
        public string NationalId { get; set; }

        [MaxLength(10)]
        public string Gender { get; set; }

        [Required]
        [Column(TypeName = "DATE")]
        public DateTime DateOfBirth { get; set; }

        [MaxLength(50)]
        public string? Address { get; set; }

        [MaxLength(50)]
        [EmailAddress]
        public string Email { get; set; }

        [Phone]
        public string Phone { get; set; }

        [Required]
        [Column(TypeName = "DATE")]
        public DateTime Join_Date { get; set; }

        [MaxLength(20)]
        public string Position { get; set; }

        [Required]
        [MaxLength(255)]
        public string ImagePath { get; set; }

        public int DepartmentId { get; set; }
        [ForeignKey(nameof(DepartmentId))]
        public Department Department { get; set; }

        public List<Class> Classes { get; set; } = new List<Class>();

       
    }
}
