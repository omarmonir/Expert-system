using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using iTextSharp.text;
using iTextSharp.text.pdf;
using iTextSharp.text.pdf.draw;

//public class PdfService
//{
//    public async Task<byte[]> GeneratePdfAsync<T>(IEnumerable<T> data, string title)
//    {
//        using var ms = new MemoryStream();
//        var document = new Document(PageSize.A4.Rotate(), 20f, 20f, 20f, 20f);
//        var writer = PdfWriter.GetInstance(document, ms);
//        writer.RunDirection = PdfWriter.RUN_DIRECTION_RTL;

//        document.AddCreator("نظام إدارة الطلاب");
//        document.AddAuthor("جامعة الفيوم");
//        document.AddSubject(title);
//        document.AddTitle(title);
//        document.Open();

//        BaseFont baseFont = null;
//        string[] fontPaths = {
//            "C:\\Windows\\Fonts\\arial.ttf",
//            "C:\\Windows\\Fonts\\tahoma.ttf",
//            "/usr/share/fonts/truetype/dejavu/DejaVuSans.ttf"
//        };

//        foreach (var path in fontPaths)
//        {
//            try
//            {
//                if (File.Exists(path))
//                {
//                    baseFont = BaseFont.CreateFont(path, BaseFont.IDENTITY_H, BaseFont.EMBEDDED);
//                    break;
//                }
//            }
//            catch { continue; }
//        }

//        if (baseFont == null)
//        {
//            baseFont = BaseFont.CreateFont(BaseFont.HELVETICA, BaseFont.CP1252, BaseFont.EMBEDDED);
//        }

//        var titleFont = new Font(baseFont, 18, Font.BOLD);
//        var subtitleFont = new Font(baseFont, 14, Font.BOLD);
//        var headerFont = new Font(baseFont, 10, Font.BOLD, BaseColor.WHITE);
//        var cellFont = new Font(baseFont, 9, Font.NORMAL);
//        var footerFont = new Font(baseFont, 8, Font.ITALIC);

//        AddHeader(document, title, titleFont, baseFont);

//        // تاريخ التقرير بتنسيق صحيح
//        var dateString = DateTime.Now.ToString("dd/MM/yyyy HH:mm", new CultureInfo("ar-EG"));
//        var dateText = "تاريخ التقرير: " + dateString;

//        var dateTable = new PdfPTable(1);
//        dateTable.RunDirection = PdfWriter.RUN_DIRECTION_RTL;
//        dateTable.WidthPercentage = 100;

//        var dateCell = new PdfPCell();
//        var dateParagraph = new Paragraph(dateText, subtitleFont);
//        dateParagraph.Alignment = Element.ALIGN_CENTER;
//        dateCell.AddElement(dateParagraph);
//        dateCell.Border = Rectangle.NO_BORDER;
//        dateCell.HorizontalAlignment = Element.ALIGN_CENTER;
//        dateTable.AddCell(dateCell);

//        document.Add(dateTable);
//        document.Add(Chunk.NEWLINE);

//        var properties = typeof(T).GetProperties()
//            .Where(p => !p.Name.Contains("ImagePath"))
//            .ToArray();

//        var table = new PdfPTable(properties.Length)
//        {
//            RunDirection = PdfWriter.RUN_DIRECTION_RTL,
//            WidthPercentage = 100,
//            SpacingBefore = 10f,
//            SpacingAfter = 10f
//        };

//        float[] columnWidths = Enumerable.Repeat(1f, properties.Length).ToArray();
//        table.SetWidths(columnWidths);

//        foreach (var prop in properties)
//        {
//            var arabicName = GetArabicColumnName(prop.Name);
//            var headerCell = new PdfPCell(new Phrase(arabicName, headerFont))
//            {
//                RunDirection = PdfWriter.RUN_DIRECTION_RTL,
//                HorizontalAlignment = Element.ALIGN_CENTER,
//                VerticalAlignment = Element.ALIGN_MIDDLE,
//                BackgroundColor = new BaseColor(41, 128, 185),
//                BorderColor = new BaseColor(52, 73, 94),
//                BorderWidth = 1f,
//                Padding = 5,
//                NoWrap = false
//            };
//            table.AddCell(headerCell);
//        }

//        bool oddRow = true;
//        foreach (var item in data)
//        {
//            foreach (var prop in properties)
//            {
//                var value = prop.GetValue(item)?.ToString() ?? string.Empty;

//                if (prop.PropertyType == typeof(DateTime) || prop.PropertyType == typeof(DateTime?))
//                {
//                    if (DateTime.TryParse(value, out DateTime date))
//                        value = date.ToString("dd/MM/yyyy", CultureInfo.InvariantCulture);
//                }
//                else if (prop.PropertyType == typeof(decimal) || prop.PropertyType == typeof(double))
//                {
//                    if (decimal.TryParse(value, out decimal number))
//                        value = number.ToString("N2", CultureInfo.InvariantCulture);
//                }

//                var cell = new PdfPCell(new Phrase(value, cellFont))
//                {
//                    RunDirection = PdfWriter.RUN_DIRECTION_RTL,
//                    HorizontalAlignment = Element.ALIGN_CENTER,
//                    VerticalAlignment = Element.ALIGN_MIDDLE,
//                    Padding = 4,
//                    NoWrap = false,
//                    BackgroundColor = oddRow ? new BaseColor(245, 245, 245) : BaseColor.WHITE,
//                    BorderColor = new BaseColor(189, 195, 199),
//                    BorderWidth = 0.5f
//                };
//                table.AddCell(cell);
//            }
//            oddRow = !oddRow;
//        }

//        document.Add(table);
//        AddFooter(document, writer, footerFont);
//        document.Close();
//        return ms.ToArray();
//    }

//    private void AddHeader(Document document, string title, Font titleFont, BaseFont baseFont)
//    {
//        var headerTable = new PdfPTable(2)
//        {
//            RunDirection = PdfWriter.RUN_DIRECTION_RTL,
//            WidthPercentage = 100
//        };
//        headerTable.SetWidths(new float[] { 3f, 1f });

//        var logoCell = new PdfPCell
//        {
//            BorderWidth = 0,
//            HorizontalAlignment = Element.ALIGN_CENTER,
//            VerticalAlignment = Element.ALIGN_MIDDLE,
//            Padding = 5
//        };

//        var logoPlaceholder = new Paragraph("شعار الجامعة", new Font(baseFont, 12, Font.BOLD))
//        {
//            Alignment = Element.ALIGN_CENTER
//        };
//        logoCell.AddElement(logoPlaceholder);

//        var titleCell = new PdfPCell
//        {
//            BorderWidth = 0,
//            HorizontalAlignment = Element.ALIGN_CENTER,
//            VerticalAlignment = Element.ALIGN_MIDDLE,
//            PaddingBottom = 15,
//            PaddingTop = 15
//        };

//        // تغيير اسم الجامعة إلى جامعة الفيوم
//        var universityParagraph = new Paragraph("جامعة الفيوم", new Font(baseFont, 20, Font.BOLD))
//        {
//            Alignment = Element.ALIGN_CENTER,
//            SpacingAfter = 10f
//        };
//        titleCell.AddElement(universityParagraph);

//        // كلية العلوم
//        var collegeParagraph = new Paragraph("كلية العلوم", new Font(baseFont, 16, Font.BOLD))
//        {
//            Alignment = Element.ALIGN_CENTER,
//            SpacingAfter = 10f
//        };
//        titleCell.AddElement(collegeParagraph);

//        // عنوان التقرير
//        var titleParagraph = new Paragraph(title, titleFont)
//        {
//            Alignment = Element.ALIGN_CENTER,
//            SpacingAfter = 5f
//        };
//        titleCell.AddElement(titleParagraph);

//        headerTable.AddCell(titleCell);
//        headerTable.AddCell(logoCell);

//        document.Add(headerTable);

//        var lineSeparator = new LineSeparator(1f, 100f, new BaseColor(52, 73, 94), Element.ALIGN_CENTER, -10);
//        document.Add(new Chunk(lineSeparator));
//        document.Add(Chunk.NEWLINE);
//    }

//    private void AddFooter(Document document, PdfWriter writer, Font footerFont)
//    {
//        var lineSeparator = new LineSeparator(1f, 100f, new BaseColor(52, 73, 94), Element.ALIGN_CENTER, -10);
//        document.Add(new Chunk(lineSeparator));

//        var footerTable = new PdfPTable(3)
//        {
//            RunDirection = PdfWriter.RUN_DIRECTION_RTL,
//            WidthPercentage = 100,
//            SpacingBefore = 10f
//        };
//        footerTable.SetWidths(new float[] { 1f, 1f, 1f });

//        var cell1 = new PdfPCell(new Phrase("نظام إدارة بيانات الطلاب", footerFont))
//        {
//            BorderWidth = 0,
//            HorizontalAlignment = Element.ALIGN_RIGHT
//        };

//        var pageNumber = new PdfPCell(new Phrase($"صفحة {writer.PageNumber}", footerFont))
//        {
//            BorderWidth = 0,
//            HorizontalAlignment = Element.ALIGN_CENTER
//        };

//        // تاريخ التقرير بتنسيق صحيح
//        var cell3 = new PdfPCell(new Phrase(DateTime.Now.ToString("dd/MM/yyyy", new CultureInfo("ar-EG")), footerFont))
//        {
//            BorderWidth = 0,
//            HorizontalAlignment = Element.ALIGN_LEFT
//        };

//        footerTable.AddCell(cell1);
//        footerTable.AddCell(pageNumber);
//        footerTable.AddCell(cell3);

//        document.Add(footerTable);
//    }

//    private string GetArabicColumnName(string englishName)
//    {
//        var translations = new Dictionary<string, string>
//        {
//            { "Id", "الرقم" },
//            { "Name", "الاسم" },
//            { "NationalId", "الرقم القومي" },
//            { "Gender", "الجنس" },
//            { "Address", "العنوان" },
//            { "Nationality", "الجنسية" },
//            { "Email", "البريد الإلكتروني" },
//            { "Phone", "الهاتف" },
//            { "Semester", "الفصل الدراسي" },
//            { "DateOfBirth", "تاريخ الميلاد" },
//            { "EnrollmentDate", "تاريخ التسجيل" },
//            { "GPA_Average", "المعدل التراكمي" },
//            { "High_School_degree", "درجة الثانوية" },
//            { "High_School_Section", "قسم الثانوية" },
//            { "CreditsCompleted", "الساعات المكتملة" },
//            { "DepartmentName", "اسم القسم" },
//            { "status", "الحالة" },
//            { "StudentLevel", "المستوى الدراسي" }
//        };

//        return translations.ContainsKey(englishName) ? translations[englishName] : englishName;
//    }
//}