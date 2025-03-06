using System.ComponentModel.DataAnnotations;
using System.ComponentModel;

namespace FacultyManagementSystemAPI.Models.DTOs.Class
{
    public class CreateClassDto
    {
        [Required(ErrorMessage = "وقت البداية مطلوب.")]
        [DefaultValue("09:00:00")]
        public TimeSpan StartTime { get; set; }

        [Required(ErrorMessage = "وقت النهاية مطلوب.")]
        [DefaultValue("11:00:00")]
        public TimeSpan EndTime { get; set; }

        [Required(ErrorMessage = "اليوم مطلوب.")]
        [MaxLength(20, ErrorMessage = "يجب ألا يتجاوز اليوم 20 حرفًا.")]
        [RegularExpression(@"^(الأحد|الإثنين|الثلاثاء|الأربعاء|السبت|الخميس)$", ErrorMessage = "يجب أن يكون اليوم بين السبت والخميس.")]
        [DefaultValue("الثلاثاء")]
        public string Day { get; set; }

        [MaxLength(100, ErrorMessage = "يجب ألا يتجاوز الموقع 100 حرف.")]
        [DefaultValue("معمل 5 - مبنى الفيزياء")]
        public string Location { get; set; }

        [Required(ErrorMessage = "رقم الدكتور مطلوب.")]
        [DefaultValue(1035)]
        public int ProfessorId { get; set; }

        [Required(ErrorMessage = "رقم المادة مطلوب.")]
        [DefaultValue(10085)]
        public int CourseId { get; set; }
    }
}
