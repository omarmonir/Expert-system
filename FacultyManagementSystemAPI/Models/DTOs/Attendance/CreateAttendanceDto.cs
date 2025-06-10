using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace FacultyManagementSystemAPI.Models.DTOs.Attendance
{
    public class CreateAttendanceDto
    {
        [Required(ErrorMessage = "التاريخ مطلوب")]
        [DataType(DataType.Date, ErrorMessage = "تنسيق التاريخ غير صحيح")]
        [DefaultValue("2005-02-15")]
        public DateTime Date { get; set; }

        [Required(ErrorMessage = "حالة الحضور مطلوبة")]
        [DefaultValue(true)]
        public bool Status { get; set; }

        [Required(ErrorMessage = "معرف الطالب مطلوب")]
        [Range(1, int.MaxValue, ErrorMessage = "يجب أن يكون معرف الطالب رقمًا صالحًا.")]
        [DefaultValue(1111)]
        public int StudentId { get; set; }
        public string CourseName { get; set; }
    }
}