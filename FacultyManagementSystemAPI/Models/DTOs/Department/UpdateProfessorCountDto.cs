using System.ComponentModel.DataAnnotations;

namespace FacultyManagementSystemAPI.Models.DTOs.Department
{
    public class UpdateProfessorCountDto
    {
        [Required(ErrorMessage = "عدد الأساتذة مطلوب.")]
        [Range(1, 100, ErrorMessage = "يجب أن يكون عدد الأساتذة أكبر من 0.")]
        [DeniedValues(10)]
        public int ProfessorCount { get; set; }
    }
}
