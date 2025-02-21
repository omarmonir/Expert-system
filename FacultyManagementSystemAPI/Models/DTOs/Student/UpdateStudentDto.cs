using System.ComponentModel.DataAnnotations;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;

namespace FacultyManagementSystemAPI.Models.DTOs.Student
{
	public class UpdateStudentDto
	{
		[Required(ErrorMessage = "الاسم الكامل مطلوب")]
		[MaxLength(100, ErrorMessage = "يجب ألا يتجاوز الاسم الكامل 100 حرف")]
		[DefaultValue("الأسم كامل")]
		public string Name { get; set; }

		[MaxLength(100, ErrorMessage = "يجب ألا يتجاوز العنوان 100 حرف")]
		[DefaultValue("الجيزة، مصر")]
		public string? Address { get; set; }

		[Required(ErrorMessage = "البريد الإلكتروني مطلوب")]
		[EmailAddress(ErrorMessage = "تنسيق البريد الإلكتروني غير صحيح")]
		[MaxLength(100, ErrorMessage = "يجب ألا يتجاوز البريد الإلكتروني 100 حرف")]
		[DefaultValue("user@example.com")]
		public string Email { get; set; }

		[Required(ErrorMessage = "رقم الهاتف مطلوب")]
		[Phone(ErrorMessage = "تنسيق رقم الهاتف غير صحيح")]
		[DefaultValue("+201234567890")]
		public string Phone { get; set; }

		[Required(ErrorMessage = "الفصل الدراسي مطلوب")]
		[Range(1, 8, ErrorMessage = "يجب أن يكون الفصل الدراسي بين 1 و 8")]
		[DefaultValue(2)]
		public byte Semester { get; set; }

		[Required(ErrorMessage = "المعدل التراكمي مطلوب")]
		[Column(TypeName = "decimal(10,2)")]
		[Range(0.00, 4.00, ErrorMessage = "يجب أن يكون المعدل التراكمي بين 0.00 و 4.00")]
		[DefaultValue(3.50)]
		public decimal GPA { get; set; }

		[Required(ErrorMessage = "عدد الساعات المكتملة مطلوب")]
		[Range(0, 140, ErrorMessage = "يجب أن يكون عدد الساعات المكتملة رقمًا غير سالب")]
		[DefaultValue(50)]
		public int CreditsCompleted { get; set; }

		[Required(ErrorMessage = "الصورة مطلوبة")]
		public IFormFile Image { get; set; }

		// Navigation Properties
		[Required(ErrorMessage = "رقم القسم مطلوب")]
		[DefaultValue(1111)]
		public int DepartmentId { get; set; }
	}
}
