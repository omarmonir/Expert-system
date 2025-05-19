using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace FacultyManagementSystemAPI.Models.DTOs.Enrollment
{
    public class CreateEnrollmentDto
    {
        [Required(ErrorMessage = "رقم الطالب مطلوب")]
        [Range(1, int.MaxValue, ErrorMessage = "رقم الطالب يجب أن يكون رقمًا موجبًا")]
        [DefaultValue("1000")]
        public int StudentId { get; set; }

        [Required(ErrorMessage = "رقم الكورس مطلوب")]
        [Range(1, int.MaxValue, ErrorMessage = "رقم الكورس يجب أن يكون رقمًا موجبًا")]
        [DefaultValue("1000")]
        public int CourseId { get; set; }

        [Required(ErrorMessage = "تاريخ التسجيل مطلوب")]
        [DataType(DataType.Date, ErrorMessage = "تنسيق التاريخ غير صحيح")]
        [DefaultValue(typeof(DateTime), "2025-02-15")]
        public DateTime AddedEnrollmentDate { get; set; } = DateTime.Now;

        [Required(ErrorMessage = "رقم الفصل الدراسي مطلوب")]
        [Range(1, int.MaxValue, ErrorMessage = "رقم الفصل يجب أن يكون رقمًا موجبًا")]
        public int NumberOFSemster { get; set; }

        [Required(ErrorMessage = "الفصل الدراسي مطلوب")]
        [RegularExpression(@"^(خريف|ربيع|صيف|شتاء) \d{4}$", ErrorMessage = "يجب أن يكون الفصل الدراسي بصيغة (خريف 2025، ربيع 2025، صيف 2025)")]
        [DefaultValue("خريف 2025")]
        public string Semester { get; set; }

        //[RegularExpression(@"^(مسجل|مكتمل|ملغي)$", ErrorMessage = "يجب أن تكون حالة الكورس إما 'مسجل' أو 'مكتمل' أو 'ملغي'")]
        [Required(ErrorMessage = "حالة الكورس مطلوبة")]
        //[RegularExpression(@"^(ملغى|نشط)$", ErrorMessage = "يجب أن تكون حالة الكورس إما 'ملغى' أو 'نشط'")]
        [DefaultValue("ناجح")]
        public string IsCompleted { get; set; }
    }
}
