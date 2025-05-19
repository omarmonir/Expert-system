using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FacultyManagementSystemAPI.Models.Entities
{
    public class Course
    {
        [Key]
        public int Id { get; set; }

        [MaxLength(50)]
        public string Name { get; set; }

        [MaxLength(50)]
        public string Code { get; set; }

        [MaxLength(250)]
        public string Description { get; set; }

        [Required]
        public int Credits { get; set; }

        [MaxLength(50)]
        public string Status { get; set; }

        [Required]
        public byte Semester { get; set; }

        [Required]
        public int MaxSeats { get; set; }

        public int CurrentEnrolledStudents { get; set; } = 0;

        public int DepartmentId { get; set; }
        [ForeignKey(nameof(DepartmentId))]
        public Department Department { get; set; }

        // Navigation Properties
        public List<Class> Classes { get; set; } = new List<Class>();
        public List<Enrollment> Enrollments { get; set; } = new List<Enrollment>();
        public List<CourseDivision> CourseDivisions { get; set; } = new List<CourseDivision>();
        public List<CoursePrerequisite> Prerequisites { get; set; } = new List<CoursePrerequisite>();
        public List<CoursePrerequisite> IsPrerequisiteFor { get; set; } = new List<CoursePrerequisite>();
    }
}
