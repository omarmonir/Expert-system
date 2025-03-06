using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace FacultyManagementSystemAPI.Models.DTOs.Enrollment
{
    public class CreateEnrollmentDto
    {
        [Required(ErrorMessage = "اسم الطالب مطلوب")]
        [MaxLength(100, ErrorMessage = "يجب ألا يتجاوز الاسم 100 حرف")]
        [DefaultValue("محمد أحمد")]
        public string StudentName { get; set; }

        [Required(ErrorMessage = "اسم الكورس مطلوب")]
        [MaxLength(100, ErrorMessage = "يجب ألا يتجاوز اسم الكورس 100 حرف")]
        [DefaultValue("برمجة متقدمة")]
        public string CourseName { get; set; }

        [Required(ErrorMessage = "تاريخ التسجيل مطلوب")]
        [DataType(DataType.Date, ErrorMessage = "تنسيق التاريخ غير صحيح")]
        [DefaultValue("2025-02-15")]
        public DateTime EnrollmentDate { get; set; } = DateTime.Now;

        [Required(ErrorMessage = "الفصل الدراسي مطلوب")]
        [RegularExpression(@"^(خريف|ربيع|صيف|شتاء) \d{4}$", ErrorMessage = "يجب أن يكون الفصل الدراسي بصيغة (خريف 2025، ربيع 2025، صيف 2025)")]
        [DefaultValue("خريف 2025")]
        public string Semester { get; set; }

        [Required(ErrorMessage = "حالة الكورس مطلوبة")]
        //[RegularExpression(@"^(مسجل|مكتمل|ملغي)$", ErrorMessage = "يجب أن تكون حالة الكورس إما 'مسجل' أو 'مكتمل' أو 'ملغي'")]
        [RegularExpression(@"^(ملغى|نشط)$", ErrorMessage = "يجب أن تكون حالة الكورس إما 'مسجل' أو 'مكتمل' أو 'ملغي'")]
        [DefaultValue("نشط")]
        public string CourseStatus { get; set; }
    }
}
