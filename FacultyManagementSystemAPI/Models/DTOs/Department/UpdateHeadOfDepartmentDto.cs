using System.ComponentModel.DataAnnotations;

namespace FacultyManagementSystemAPI.Models.DTOs.Department
{
    public class UpdateHeadOfDepartmentDto
    {
        [Required(ErrorMessage = "اسم رئيس القسم مطلوب.")]
        [MaxLength(100, ErrorMessage = "يجب ألا يتجاوز اسم رئيس القسم 100 حرف.")]
        public string HeadOfDepartment { get; set; }
    }
}