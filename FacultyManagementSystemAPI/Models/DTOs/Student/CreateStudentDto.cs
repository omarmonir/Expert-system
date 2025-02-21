using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;

namespace FacultyManagementSystemAPI.Models.DTOs.Student
{
	public class CreateStudentDto
	{
		[Required(ErrorMessage = "الاسم الكامل مطلوب")]
		[MaxLength(100, ErrorMessage = "يجب ألا يتجاوز الاسم الكامل 100 حرف")]
		[DefaultValue("الأسم كامل")]
		public string Name { get; set; }

		[Required(ErrorMessage = "الرقم القومي مطلوب")]
		[StringLength(14, MinimumLength = 14, ErrorMessage = "يجب أن يكون الرقم القومي مكونًا من 14 رقمًا بالضبط")]
		[RegularExpression("^[0-9]{14}$", ErrorMessage = "يجب أن يحتوي الرقم القومي على أرقام فقط")]
		[DefaultValue("12345678901234")]
		public string NationalId { get; set; }

		[Required(ErrorMessage = "النوع مطلوب")]
		[StringLength(10, ErrorMessage = "يجب ألا يتجاوز النوع 10 أحرف")]
		[DefaultValue("ذكر")]
		[RegularExpression(@"^(ذكر|أنثى)$", ErrorMessage = "يجب أن يكون النوع 'ذكر' أو 'أنثى'")]
		public string Gender { get; set; }

		[Required(ErrorMessage = "تاريخ الميلاد مطلوب")]
		[DataType(DataType.Date, ErrorMessage = "تنسيق التاريخ غير صحيح")]
		[DefaultValue("2005-02-15")]
		public DateTime DateOfBirth { get; set; }

		[MaxLength(100, ErrorMessage = "يجب ألا يتجاوز العنوان 100 حرف")]
		[DefaultValue("الجيزة، مصر")]
		public string? Address { get; set; }

		[MaxLength(50, ErrorMessage = "يجب ألا تتجاوز الجنسية 50 حرفًا")]
		[DefaultValue("مصري")]
		public string? Nationality { get; set; }

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

		[Required(ErrorMessage = "تاريخ التسجيل مطلوب")]
		[DataType(DataType.Date, ErrorMessage = "تنسيق التاريخ غير صحيح")]
		[DefaultValue("2025-02-15")]
		public DateTime EnrollmentDate { get; set; } = DateTime.Now;

		[Required(ErrorMessage = "المعدل التراكمي مطلوب")]
		[Column(TypeName = "decimal(10,2)")]
		[Range(0.00, 4.00, ErrorMessage = "يجب أن يكون المعدل التراكمي بين 0.00 و 4.00")]
		[DefaultValue(3.50)]
		public decimal GPA { get; set; }

		[Required(ErrorMessage = "درجة الثانوية العامة مطلوبة")]
		[Column(TypeName = "decimal(10,2)")]
		[Range(205.00, 410.00, ErrorMessage = "يجب أن تكون درجة الثانوية العامة بين 205.00 و 410.00")]
		[DefaultValue(365.00)]
		public decimal High_School_degree { get; set; }

		[Required(ErrorMessage = "قسم الثانوية العامة مطلوب")]
		[MaxLength(50, ErrorMessage = "يجب ألا يتجاوز قسم الثانوية العامة 50 حرفًا")]
		[DefaultValue("رياضيات")]
		public string High_School_Section { get; set; }

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
