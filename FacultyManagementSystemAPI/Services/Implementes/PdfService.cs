using FacultyManagementSystemAPI.Models.DTOs.Report;
using iTextSharp.text.pdf;
using iTextSharp.text;

namespace FacultyManagementSystemAPI.Services.Implementes
{
    public class PdfService
    {
        public async Task<byte[]> GeneratePdfAsync<T>(IEnumerable<T> data, string title)
        {
            using var ms = new MemoryStream();
            var document = new Document(PageSize.A4);
            var writer = PdfWriter.GetInstance(document, ms);
            document.Open();

            // 🔹 تحميل خط يدعم العربية
            string fontPath = "C:\\Windows\\Fonts\\arial.ttf"; // تأكد من أن الخط موجود على جهازك
            BaseFont baseFont = BaseFont.CreateFont(fontPath, BaseFont.IDENTITY_H, BaseFont.EMBEDDED);
            Font arabicFont = new Font(baseFont, 14, Font.NORMAL);

            // 🔹 إضافة العنوان بتنسيق عربي
            var titlePhrase = new Paragraph(title, new Font(baseFont, 18, Font.BOLD))
            {
                Alignment = Element.ALIGN_CENTER
            };
            document.Add(titlePhrase);
            document.Add(new Chunk("\n"));

            // 🔹 إنشاء الجدول مع دعم اللغة العربية
            var properties = typeof(T).GetProperties();
            var table = new PdfPTable(properties.Length)
            {
                RunDirection = PdfWriter.RUN_DIRECTION_RTL,  // 🟢 دعم الكتابة من اليمين إلى اليسار
                WidthPercentage = 100
            };

            // 🔹 إضافة رؤوس الأعمدة مع دعم العربية
            foreach (var prop in properties)
            {
                PdfPCell cell = new PdfPCell(new Phrase(prop.Name, arabicFont))
                {
                    HorizontalAlignment = Element.ALIGN_CENTER,
                    RunDirection = PdfWriter.RUN_DIRECTION_RTL
                };
                table.AddCell(cell);
            }

            // 🔹 إضافة البيانات إلى الجدول
            foreach (var item in data)
            {
                foreach (var prop in properties)
                {
                    var value = prop.GetValue(item)?.ToString() ?? string.Empty;
                    PdfPCell cell = new PdfPCell(new Phrase(value, arabicFont))
                    {
                        HorizontalAlignment = Element.ALIGN_CENTER,
                        RunDirection = PdfWriter.RUN_DIRECTION_RTL
                    };
                    table.AddCell(cell);
                }
            }

            document.Add(table);
            document.Close();
            return await Task.FromResult(ms.ToArray());
        }


    }
}
