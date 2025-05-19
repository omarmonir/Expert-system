using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace FacultyManagementSystemAPI.Models.DTOs.professors
{
    public class CreateProfessorDto
    {
        [Required(ErrorMessage = "الاسم مطلوب")]
        [MaxLength(100, ErrorMessage = "يجب ألا يتجاوز الاسم 100 حرف")]
        [DefaultValue("اسم الدكتور")]
        public string FullName { get; set; }

        [Required(ErrorMessage = "الرقم القومي مطلوب")]
        [StringLength(14, MinimumLength = 14, ErrorMessage = "يجب أن يكون الرقم القومي مكونًا من 14 رقمًا بالضبط")]
        [RegularExpression("^[0-9]{14}$", ErrorMessage = "يجب أن يحتوي الرقم القومي على أرقام فقط")]
        [DefaultValue("12345678901234")]
        public string NationalId { get; set; }

        [Required(ErrorMessage = "الجنس مطلوب")]
        [StringLength(10, ErrorMessage = "يجب ألا يتجاوز الجنس 10 أحرف")]
        [DefaultValue("ذكر")]
        [RegularExpression(@"^(ذكر|أنثى)$", ErrorMessage = "يجب أن يكون النوع 'ذكر' أو 'أنثى'")]
        public string Gender { get; set; }

        [Required(ErrorMessage = "تاريخ الميلاد مطلوب")]
        [DataType(DataType.Date, ErrorMessage = "تنسيق التاريخ غير صحيح")]
        [DefaultValue("1980-01-15")]
        public DateTime DateOfBirth { get; set; }

        [MaxLength(100, ErrorMessage = "يجب ألا يتجاوز العنوان 100 حرف")]
        [DefaultValue("القاهرة، مصر")]
        public string? Address { get; set; }

        [Required(ErrorMessage = "البريد الإلكتروني مطلوب")]
        [EmailAddress(ErrorMessage = "تنسيق البريد الإلكتروني غير صحيح")]
        [MaxLength(100, ErrorMessage = "يجب ألا يتجاوز البريد الإلكتروني 100 حرف")]
        [DefaultValue("professor@example.com")]
        public string Email { get; set; }

        [Required(ErrorMessage = "رقم الهاتف مطلوب")]
        [Phone(ErrorMessage = "تنسيق رقم الهاتف غير صحيح")]
        [DefaultValue("+201234567890")]
        public string Phone { get; set; }

        [Required(ErrorMessage = "تاريخ الانضمام مطلوب")]
        [DataType(DataType.Date, ErrorMessage = "تنسيق التاريخ غير صحيح")]
        [DefaultValue("2020-09-01")]
        public DateTime JoinDate { get; set; }

        [Required(ErrorMessage = "المسمى الوظيفي مطلوب")]
        [MaxLength(20, ErrorMessage = "يجب ألا يتجاوز المسمى الوظيفي 20 حرفًا")]
        [DefaultValue("أستاذ مساعد")]
        public string Position { get; set; }

        [DefaultValue("default.jpg")]
        public IFormFile? Image { get; set; }

        // Navigation Properties
        [Required(ErrorMessage = "اسم القسم مطلوب")]
        [MaxLength(100, ErrorMessage = "يجب ألا يتجاوز اسم القسم 100 حرف")]
        [DefaultValue("قسم الرياضيات")]
        public string DepartmentName { get; set; }
    }
}
