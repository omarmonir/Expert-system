using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace FacultyManagementSystemAPI.Models.DTOs.Courses
{
    public class CreateCourseDto
    {
        [Required(ErrorMessage = "اسم المقرر مطلوب.")]
        [MaxLength(50, ErrorMessage = "يجب ألا يتجاوز اسم المقرر 50 حرفاً.")]
        [DefaultValue("أسم المادة")]
        public string Name { get; set; }

        [Required(ErrorMessage = "كود المقرر مطلوب.")]
        [MaxLength(50, ErrorMessage = "يجب ألا يتجاوز كود المقرر 50 حرفاً.")]
        [DefaultValue("CS102")]
        public string Code { get; set; }

        [Required(ErrorMessage = "الوصف مطلوب.")]
        [MaxLength(250, ErrorMessage = "يجب ألا يتجاوز اسم الوصف 250 حرفاً.")]
        [DefaultValue("الوصف")]
        public string Description { get; set; }

        [Required(ErrorMessage = "عدد الساعات المعتمده مطلوب.")]
        [Range(1, 10, ErrorMessage = "يجب أن يكون عدد الساعات بين 1 و10.")]
        [DefaultValue(3)]
        public int Credits { get; set; }

        [Required(ErrorMessage = "حالة المقرر مطلوبة.")]
        [MaxLength(50, ErrorMessage = "يجب ألا تتجاوز حالة المقرر 50 حرفاً.")]
        [RegularExpression(@"^(غير متاح|متاح)$", ErrorMessage = "يجب أن يكون النوع 'ذكر' أو 'أنثى'")]
        [DefaultValue("غير متاح")]
        public string Status { get; set; }

        [Required(ErrorMessage = "الفصل الدراسي مطلوب.")]
        [Range(1, 8, ErrorMessage = "يجب أن يكون الفصل الدراسي بين 1 و8.")]
        [DefaultValue(5)]
        public byte Semester { get; set; }

        [DefaultValue(1111)]
        public int? PreCourseId { get; set; }
    }
}
