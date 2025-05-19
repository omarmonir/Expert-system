using System.ComponentModel.DataAnnotations;

namespace FacultyManagementSystemAPI.Models.Entities
{
    public class EnrollmentPeriod
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(250)]
        public string Semester { get; set; }


        [Required]
        public DateTime StartDate { get; set; }

        [Required]
        public DateTime EndDate { get; set; }
    }
}
