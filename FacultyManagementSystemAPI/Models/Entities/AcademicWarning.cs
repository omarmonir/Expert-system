using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FacultyManagementSystemAPI.Models.Entities
{
    public class AcademicWarning
    {
        [Key]
        public int Id { get; set; }

        // المفتاح الخارجي مع تعريف واضح
        public int StudentId { get; set; }

        [Required]
        [StringLength(50)]
        public string WarningType { get; set; }

        [Required]
        public int WarningLevel { get; set; }

        [StringLength(255)]
        public string Description { get; set; }

        [StringLength(50)]
        public string Semester { get; set; }

        [Required]
        public DateTime IssueDate { get; set; }

        public DateTime? ResolvedDate { get; set; }

        [Required]
        [StringLength(20)]
        public string Status { get; set; } = "Active";

        [StringLength(255)]
        public string ActionRequired { get; set; }

        public string Notes { get; set; }

        // علاقة التنقل مع تحديد المفتاح الخارجي
        [ForeignKey(nameof(StudentId))]
        public virtual Student Student { get; set; }
    }
}
