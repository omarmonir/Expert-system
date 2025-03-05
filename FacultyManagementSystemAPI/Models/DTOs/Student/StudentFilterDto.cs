namespace FacultyManagementSystemAPI.Models.DTOs.Student
{
    using System;
    using System.ComponentModel.DataAnnotations;

    public class StudentFilterDto
    {
        [MaxLength(100, ErrorMessage = "يجب ألا يتجاوز الاسم 100 حرف.")]
        public string? Name { get; set; }

        [StringLength(14, MinimumLength = 14, ErrorMessage = "يجب أن يكون الرقم القومي مكونًا من 14 رقمًا بالضبط.")]
        public string? NationalId { get; set; }

        [MaxLength(100, ErrorMessage = "يجب ألا يتجاوز البريد الإلكتروني 100 حرف.")]
        [EmailAddress(ErrorMessage = "تنسيق البريد الإلكتروني غير صالح.")]
        public string? Email { get; set; }

        [MaxLength(10, ErrorMessage = "يجب ألا يتجاوز النوع 10 أحرف.")]
        [RegularExpression(@"^(ذكر|أنثى)$", ErrorMessage = "يجب أن يكون النوع 'ذكر' أو 'أنثى'")]
        public string? Gender { get; set; }

        [RegularExpression(@"^(010|011|012|015)\d{8}$", ErrorMessage = "يجب أن يكون رقم الهاتف مصريًا مكونًا من 11 رقمًا ويبدأ بـ 010 أو 011 أو 012 أو 015.")]
        public string? Phone { get; set; }

        [Range(1, 8, ErrorMessage = "يجب أن يكون الفصل الدراسي رقمًا موجبًا أكبر من الصفر.")]
        public byte? Semester { get; set; }

        [Range(205, 410, ErrorMessage = "يجب أن تكون درجة الثانوية بين 205 و 410.")]
        public decimal? MaxHigh_School_degree { get; set; }

        [Range(205, 410, ErrorMessage = "يجب أن تكون درجة الثانوية بين 205 و 410.")]
        public decimal? MinHigh_School_degree { get; set; }

        [MaxLength(50, ErrorMessage = "يجب ألا يتجاوز قسم الثانوية 50 حرفًا.")]
        public string? High_School_Section { get; set; }

        [DataType(DataType.Date, ErrorMessage = "تنسيق تاريخ الالتحاق غير صالح.")]
        public DateTime? MaxEnrollmentDate { get; set; }

        [DataType(DataType.Date, ErrorMessage = "تنسيق تاريخ الالتحاق غير صالح.")]
        public DateTime? MinEnrollmentDate { get; set; }

        [DataType(DataType.Date, ErrorMessage = "تنسيق تاريخ الميلاد الأدنى غير صالح.")]
        public DateTime? MinDateOfBirth { get; set; }

        [DataType(DataType.Date, ErrorMessage = "تنسيق تاريخ الميلاد الأقصى غير صالح.")]
        public DateTime? MaxDateOfBirth { get; set; }

        [Range(0.0, 4.0, ErrorMessage = "يجب أن يكون المعدل التراكمي بين 0.0 و 4.0.")]
        public decimal? MinGPA { get; set; }

        [Range(0.0, 4.0, ErrorMessage = "يجب أن يكون المعدل التراكمي بين 0.0 و 4.0.")]
        public decimal? MaxGPA { get; set; }

        [Range(1, 10000, ErrorMessage = "يجب أن يكون الرقم التعريفى للقسم رقمًا موجبًا أكبر من الصفر.")]
        public int? DepartmentId { get; set; }
        public string? SortBy { get; set; }
        public string? SortOrder { get; set; } = "ASC";
    }

}