using System.ComponentModel.DataAnnotations;
using System.ComponentModel;

namespace FacultyManagementSystemAPI.Models.DTOs.Auth
{
    public class UserCreateDto
    {
        [Required(ErrorMessage = "الاسم الكامل مطلوب")]
        [MaxLength(100, ErrorMessage = "يجب ألا يتجاوز الاسم الكامل 100 حرف")]
        [DefaultValue("الأسم كامل")]
        public string Name { get; set; }

        [Required(ErrorMessage = "رقم الهاتف مطلوب")]
        [Phone(ErrorMessage = "تنسيق رقم الهاتف غير صحيح")]
        [DefaultValue("+201234567890")]
        public string Phone { get; set; }

        [Required(ErrorMessage = "البريد الإلكتروني مطلوب")]
        [EmailAddress(ErrorMessage = "تنسيق البريد الإلكتروني غير صحيح")]
        [MaxLength(100, ErrorMessage = "يجب ألا يتجاوز البريد الإلكتروني 100 حرف")]
        [DefaultValue("user@gmail.com")]
        public string Email { get; set; }

      

    }
}
