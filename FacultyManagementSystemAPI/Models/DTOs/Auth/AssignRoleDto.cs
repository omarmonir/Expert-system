using System.ComponentModel.DataAnnotations;
using System.ComponentModel;

namespace FacultyManagementSystemAPI.Models.DTOs.Auth
{
    public class AssignRoleDto
    {
        [Required(ErrorMessage = "Email is required.")]
        [EmailAddress(ErrorMessage = "Invalid email format.")]
        [DefaultValue("user@example.com")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Role is required.")]
        [DefaultValue("User")]
        public string Role { get; set; }
    }
}
