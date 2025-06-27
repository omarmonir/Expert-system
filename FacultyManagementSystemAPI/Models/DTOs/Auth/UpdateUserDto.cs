using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace FacultyManagementSystemAPI.Models.DTOs.Auth
{
    public class UpdateUserDto
    {
        [Required(ErrorMessage = "الاسم الكامل مطلوب")]
        [MaxLength(100, ErrorMessage = "يجب ألا يتجاوز الاسم الكامل 100 حرف")]
        [DefaultValue("الأسم كامل")]
        public string Name { get; set; }

        [EmailAddress(ErrorMessage = "Invalid email format.")]
        [MaxLength(100, ErrorMessage = "Email cannot exceed 100 characters.")]
        [DefaultValue("user@example.com")]
        public string Email { get; set; }

        [Phone(ErrorMessage = "Invalid phone number format.")]
        [MaxLength(15, ErrorMessage = "Phone Number cannot exceed 15 characters.")]
        [DefaultValue("+201234567891")]
        public string PhoneNumber { get; set; }
    }
}
