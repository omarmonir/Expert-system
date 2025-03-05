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

        [Required(ErrorMessage = "الوصف مطلوب.")]
        [MaxLength(250, ErrorMessage = "يجب ألا يتجاوز الوصف 250 حرفاً.")]
        [DefaultValue("الوصف")]
        public string Description { get; set; }

        [Required(ErrorMessage = "عدد الساعات المعتمدة مطلوب.")]
        [Range(1, 10, ErrorMessage = "يجب أن يكون عدد الساعات بين 1 و10.")]
        [DefaultValue(3)]
        public int Credits { get; set; }

        [Required(ErrorMessage = "حالة المقرر مطلوبة.")]
        [MaxLength(50, ErrorMessage = "يجب ألا تتجاوز حالة المقرر 50 حرفاً.")]
        [RegularExpression(@"^(غير متاح|متاح)$", ErrorMessage = "يجب أن تكون الحالة 'غير متاح' أو 'متاح'.")]
        [DefaultValue("غير متاح")]
        public string Status { get; set; }

        [Required(ErrorMessage = "الفصل الدراسي مطلوب.")]
        [Range(1, 8, ErrorMessage = "يجب أن يكون الفصل الدراسي بين 1 و8.")]
        [DefaultValue(5)]
        public byte Semester { get; set; }

        public int? PreCourseId { get; set; }

        // الإضافات الجديدة
        [Required(ErrorMessage = "الحد الأقصى للمقاعد مطلوب.")]
        [Range(1, 500, ErrorMessage = "يجب أن يكون الحد الأقصى للمقاعد بين 1 و500.")]
        [DefaultValue(50)]
        public int MaxSeats { get; set; }

        [Required(ErrorMessage = "عدد الطلاب المسجلين مطلوب.")]
        [Range(0, 500, ErrorMessage = "يجب أن يكون عدد الطلاب المسجلين بين 0 والحد الأقصى للمقاعد.")]
        [DefaultValue(0)]
        public int CurrentEnrolledStudents { get; set; } = 0;
    }

}
