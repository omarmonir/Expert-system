using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace FacultyManagementSystemAPI.Models.DTOs.Classes
{
    public class UpdateClassDto
    {
      
        [DefaultValue("09:00:00")]
        public TimeSpan StartTime { get; set; }

       
        [DefaultValue("11:00:00")]
        public TimeSpan EndTime { get; set; }

        
        [MaxLength(20, ErrorMessage = "يجب ألا يتجاوز اليوم 20 حرفًا.")]
        [RegularExpression(@"^(الأحد|الإثنين|الثلاثاء|الأربعاء|السبت|الخميس)$", ErrorMessage = "يجب أن يكون اليوم بين السبت والخميس.")]
        [DefaultValue("الثلاثاء")]
        public string Day { get; set; }

        [MaxLength(100, ErrorMessage = "يجب ألا يتجاوز الموقع 100 حرف.")]
        [DefaultValue("معمل 5 - مبنى الفيزياء")]
        public string Location { get; set; }

       
        [DefaultValue(1035)]
        public int ProfessorId { get; set; }

        
        [DefaultValue(10085)]
        public int CourseId { get; set; }
    }
}
