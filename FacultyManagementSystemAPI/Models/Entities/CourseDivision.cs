using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FacultyManagementSystemAPI.Models.Entities
{
    public class CourseDivision
    {
        [Key]
        public int Id { get; set; }

        public int CourseId { get; set; }
        [ForeignKey(nameof(CourseId))]
        public Course Course { get; set; }

        public int DivisionId { get; set; }
        [ForeignKey(nameof(DivisionId))]
        public Division Division { get; set; }

        [Required]
        public bool IsMandatory { get; set; }
    }
}
