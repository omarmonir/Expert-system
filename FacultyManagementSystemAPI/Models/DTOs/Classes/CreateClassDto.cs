using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace FacultyManagementSystemAPI.Models.DTOs.Classes
{

    public class CreateClassByNameDto
    {
        [Required(ErrorMessage = "وقت البداية مطلوب.")]
        public TimeSpan StartTime { get; set; }

        [Required(ErrorMessage = "وقت النهاية مطلوب.")]
        public TimeSpan EndTime { get; set; }

        [Required(ErrorMessage = "اليوم مطلوب.")]
        [MaxLength(20)]
        [RegularExpression(@"^(السبت|الأحد|الإثنين|الثلاثاء|الأربعاء|الخميس)$")]
        public string Day { get; set; }

        [MaxLength(100)]
        public string Location { get; set; }

        [Required(ErrorMessage = "اسم الدكتور مطلوب.")]
        public string ProfessorName { get; set; }

        [Required(ErrorMessage = "اسم المادة مطلوب.")]
        public string CourseName { get; set; }
    }

}
