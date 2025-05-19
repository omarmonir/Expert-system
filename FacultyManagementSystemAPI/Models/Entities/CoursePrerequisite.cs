using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FacultyManagementSystemAPI.Models.Entities
{
    public class CoursePrerequisite
    {
        [Key]
        public int Id { get; set; }

        public int CourseId { get; set; }
        [ForeignKey(nameof(CourseId))]
        public Course Course { get; set; }

        public int PrerequisiteCourseId { get; set; }
        [ForeignKey(nameof(PrerequisiteCourseId))]
        public Course PrerequisiteCourse { get; set; }
    }
}
