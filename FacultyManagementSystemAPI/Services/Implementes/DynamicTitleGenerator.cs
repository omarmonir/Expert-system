public partial class PdfService
{
    public class DynamicTitleGenerator
    {
        private readonly Dictionary<string, string> _arabicLabels;

        public DynamicTitleGenerator()
        {
            // قاموس للترجمة من الإنجليزية للعربية
            _arabicLabels = new Dictionary<string, string>
        {
            // للطلاب
            {"studentName", "الطالب"},
            {"studentStatus", "حالة الطالب"},
            {"departmentName", "القسم"},
            {"divisionName", "الشعبة"},
            
            // للكورسات
            {"courseName", "الكورس"},
            {"courseStatus", "حالة الكورس"},
            {"instructorName", "المدرس"},
            {"semester", "الفصل الدراسي"},
            {"level", "الفرقة الدراسية"},
            
            // للمحاضرات
            {"lectureName", "المحاضرة"},
            {"lectureDate", "تاريخ المحاضرة"},
            {"lectureHall", "قاعة المحاضرة"},
            
            // عام
            {"date", "التاريخ"},
            {"status", "الحالة"},
            {"type", "النوع"},
            {"category", "الفئة"},
           
            {"year", "السنة"},
            {"month", "الشهر"},
            {"week", "الأسبوع"}
        };
        }

        public string GenerateTitle(string baseTitle, Dictionary<string, object> filters)
        {
            if (filters == null || !filters.Any())
                return baseTitle;

            var title = baseTitle;
            var details = new List<string>();

            foreach (var filter in filters)
            {
                if (filter.Value != null && !string.IsNullOrEmpty(filter.Value.ToString()))
                {
                    var arabicLabel = GetArabicLabel(filter.Key);
                    var value = filter.Value.ToString();

                    // إذا كان الـ key يحتوي على Name فهو اسم شخص أو شيء محدد
                    if (filter.Key.ToLower().Contains("name"))
                    {
                        details.Add($"{arabicLabel}: {value}");
                    }
                    else
                    {
                        details.Add($"{arabicLabel}: {value}");
                    }
                }
            }

            if (details.Any())
            {
                title += " - " + string.Join(" | ", details);
            }

            return title;
        }

        public string GenerateFileName(string baseFileName, Dictionary<string, object> filters)
        {
            if (filters == null || !filters.Any())
                return $"{baseFileName}.pdf";

            var fileNameParts = new List<string> { baseFileName };

            foreach (var filter in filters)
            {
                if (filter.Value != null && !string.IsNullOrEmpty(filter.Value.ToString()))
                {
                    var cleanValue = CleanFileName(filter.Value.ToString());
                    if (!string.IsNullOrEmpty(cleanValue))
                    {
                        fileNameParts.Add(cleanValue);
                    }
                }
            }

            return string.Join("_", fileNameParts) + ".pdf";
        }

        private string GetArabicLabel(string key)
        {
            return _arabicLabels.ContainsKey(key) ? _arabicLabels[key] : key;
        }

        private string CleanFileName(string fileName)
        {
            var invalidChars = Path.GetInvalidFileNameChars();
            foreach (var invalidChar in invalidChars)
            {
                fileName = fileName.Replace(invalidChar, '_');
            }

            return fileName.Trim().Replace(" ", "_");
        }
    }
}