using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using FacultyManagementSystemAPI.Models.DTOs.Classes;
using iTextSharp.text;
using iTextSharp.text.pdf;
using iTextSharp.text.pdf.draw;

public partial class PdfService
{
    // مسارات الصور - يمكنك تعديلها حسب مكان الصور في مشروعك
    private readonly string _universityLogoPath;
    private readonly string _collegeLogoPath;

    public PdfService(string universityLogoPath = null, string collegeLogoPath = null)
    {
        _universityLogoPath = universityLogoPath ?? Path.Combine("wwwroot", "images", "university-logo.jpg");
        _collegeLogoPath = collegeLogoPath ?? Path.Combine("wwwroot", "images", "college-logo.jpg");
    }

    public async Task<byte[]> GeneratePdfAsync<T>(IEnumerable<T> data, string title)
    {
        using var ms = new MemoryStream();
        var document = new Document(PageSize.A4.Rotate(), 20f, 20f, 20f, 20f);
        var writer = PdfWriter.GetInstance(document, ms);
        writer.RunDirection = PdfWriter.RUN_DIRECTION_RTL;

        document.AddCreator("نظام إدارة الطلاب");
        document.AddAuthor("جامعة الفيوم");
        document.AddSubject(title);
        document.AddTitle(title);
        document.Open();

        BaseFont baseFont = null;
        string[] fontPaths = {
            "C:\\Windows\\Fonts\\arial.ttf",
            "C:\\Windows\\Fonts\\tahoma.ttf",
            "/usr/share/fonts/truetype/dejavu/DejaVuSans.ttf"
        };

        foreach (var path in fontPaths)
        {
            try
            {
                if (File.Exists(path))
                {
                    baseFont = BaseFont.CreateFont(path, BaseFont.IDENTITY_H, BaseFont.EMBEDDED);
                    break;
                }
            }
            catch { continue; }
        }

        if (baseFont == null)
        {
            baseFont = BaseFont.CreateFont(BaseFont.HELVETICA, BaseFont.CP1252, BaseFont.EMBEDDED);
        }

        var titleFont = new Font(baseFont, 18, Font.BOLD);
        var subtitleFont = new Font(baseFont, 14, Font.BOLD);
        var headerFont = new Font(baseFont, 10, Font.BOLD, BaseColor.WHITE);
        var cellFont = new Font(baseFont, 9, Font.NORMAL);
        var footerFont = new Font(baseFont, 8, Font.ITALIC);

        AddHeader(document, title, titleFont, baseFont);

        // تاريخ التقرير بتنسيق صحيح
        var dateString = DateTime.Now.ToString("dd/MM/yyyy HH:mm", new CultureInfo("ar-EG"));
        var dateText = "تاريخ التقرير: " + dateString;

        var dateTable = new PdfPTable(1);
        dateTable.RunDirection = PdfWriter.RUN_DIRECTION_RTL;
        dateTable.WidthPercentage = 100;

        var dateCell = new PdfPCell();
        var dateParagraph = new Paragraph(dateText, subtitleFont);
        dateParagraph.Alignment = Element.ALIGN_CENTER;
        dateCell.AddElement(dateParagraph);
        dateCell.Border = Rectangle.NO_BORDER;
        dateCell.HorizontalAlignment = Element.ALIGN_CENTER;
        dateTable.AddCell(dateCell);

        document.Add(dateTable);
        document.Add(Chunk.NEWLINE);

        var properties = typeof(T).GetProperties()
            .Where(p => !p.Name.Contains("ImagePath"))
            .ToArray();

        var table = new PdfPTable(properties.Length)
        {
            RunDirection = PdfWriter.RUN_DIRECTION_RTL,
            WidthPercentage = 100,
            SpacingBefore = 10f,
            SpacingAfter = 10f
        };

        float[] columnWidths = Enumerable.Repeat(1f, properties.Length).ToArray();
        table.SetWidths(columnWidths);

        foreach (var prop in properties)
        {
            var arabicName = GetArabicColumnName(prop.Name);
            var headerCell = new PdfPCell(new Phrase(arabicName, headerFont))
            {
                RunDirection = PdfWriter.RUN_DIRECTION_RTL,
                HorizontalAlignment = Element.ALIGN_CENTER,
                VerticalAlignment = Element.ALIGN_MIDDLE,
                BackgroundColor = new BaseColor(41, 128, 185),
                BorderColor = new BaseColor(52, 73, 94),
                BorderWidth = 1f,
                Padding = 5,
                NoWrap = false
            };
            table.AddCell(headerCell);
        }

        bool oddRow = true;
        foreach (var item in data)
        {
            foreach (var prop in properties)
            {
                var value = prop.GetValue(item)?.ToString() ?? string.Empty;

                if (prop.PropertyType == typeof(DateTime) || prop.PropertyType == typeof(DateTime?))
                {
                    if (DateTime.TryParse(value, out DateTime date))
                        value = date.ToString("dd/MM/yyyy", CultureInfo.InvariantCulture);
                }
                else if (prop.PropertyType == typeof(decimal) || prop.PropertyType == typeof(double))
                {
                    if (decimal.TryParse(value, out decimal number))
                        value = number.ToString("N2", CultureInfo.InvariantCulture);
                }

                var cell = new PdfPCell(new Phrase(value, cellFont))
                {
                    RunDirection = PdfWriter.RUN_DIRECTION_RTL,
                    HorizontalAlignment = Element.ALIGN_CENTER,
                    VerticalAlignment = Element.ALIGN_MIDDLE,
                    Padding = 4,
                    NoWrap = false,
                    BackgroundColor = oddRow ? new BaseColor(245, 245, 245) : BaseColor.WHITE,
                    BorderColor = new BaseColor(189, 195, 199),
                    BorderWidth = 0.5f
                };
                table.AddCell(cell);
            }
            oddRow = !oddRow;
        }

        document.Add(table);
        AddFooter(document, writer, footerFont);
        document.Close();
        return ms.ToArray();
    }

    private void AddHeader(Document document, string title, Font titleFont, BaseFont baseFont)
    {
        // جدول للهيدر مع 3 أعمدة (شعار الكلية، النصوص، شعار الجامعة)
        var headerTable = new PdfPTable(3)
        {
            RunDirection = PdfWriter.RUN_DIRECTION_RTL,
            WidthPercentage = 100
        };
        headerTable.SetWidths(new float[] { 1f, 3f, 1f });

        // خلية شعار الكلية (العمود الأول)
        var collegeLogoCell = new PdfPCell
        {
            BorderWidth = 0,
            HorizontalAlignment = Element.ALIGN_CENTER,
            VerticalAlignment = Element.ALIGN_MIDDLE,
            Padding = 10
        };

        if (File.Exists(_collegeLogoPath))
        {
            try
            {
                var collegeLogoImage = Image.GetInstance(_collegeLogoPath);
                collegeLogoImage.ScaleToFit(60f, 60f); // تحديد حجم الصورة
                collegeLogoImage.Alignment = Element.ALIGN_CENTER;
                collegeLogoCell.AddElement(collegeLogoImage);
            }
            catch
            {
                // في حالة فشل تحميل الصورة، اعرض نص بديل
                var collegePlaceholder = new Paragraph("شعار الكلية", new Font(baseFont, 10, Font.BOLD))
                {
                    Alignment = Element.ALIGN_CENTER
                };
                collegeLogoCell.AddElement(collegePlaceholder);
            }
        }
        else
        {
            var collegePlaceholder = new Paragraph("شعار الكلية", new Font(baseFont, 10, Font.BOLD))
            {
                Alignment = Element.ALIGN_CENTER
            };
            collegeLogoCell.AddElement(collegePlaceholder);
        }

        // خلية النصوص (العمود الأوسط)
        var titleCell = new PdfPCell
        {
            BorderWidth = 0,
            HorizontalAlignment = Element.ALIGN_CENTER,
            VerticalAlignment = Element.ALIGN_MIDDLE,
            PaddingBottom = 15,
            PaddingTop = 15
        };

        // جامعة الفيوم
        var universityParagraph = new Paragraph("جامعة الفيوم", new Font(baseFont, 20, Font.BOLD))
        {
            Alignment = Element.ALIGN_CENTER,
            SpacingAfter = 10f
        };
        titleCell.AddElement(universityParagraph);

        // كلية العلوم
        var collegeParagraph = new Paragraph("كلية العلوم", new Font(baseFont, 16, Font.BOLD))
        {
            Alignment = Element.ALIGN_CENTER,
            SpacingAfter = 10f
        };
        titleCell.AddElement(collegeParagraph);

        // عنوان التقرير
        var titleParagraph = new Paragraph(title, titleFont)
        {
            Alignment = Element.ALIGN_CENTER,
            SpacingAfter = 5f
        };
        titleCell.AddElement(titleParagraph);

        // خلية شعار الجامعة (العمود الثالث)
        var universityLogoCell = new PdfPCell
        {
            BorderWidth = 0,
            HorizontalAlignment = Element.ALIGN_CENTER,
            VerticalAlignment = Element.ALIGN_MIDDLE,
            Padding = 10
        };

        if (File.Exists(_universityLogoPath))
        {
            try
            {
                var universityLogoImage = Image.GetInstance(_universityLogoPath);
                universityLogoImage.ScaleToFit(60f, 60f); // تحديد حجم الصورة
                universityLogoImage.Alignment = Element.ALIGN_CENTER;
                universityLogoCell.AddElement(universityLogoImage);
            }
            catch
            {
                // في حالة فشل تحميل الصورة، اعرض نص بديل
                var universityPlaceholder = new Paragraph("شعار الجامعة", new Font(baseFont, 10, Font.BOLD))
                {
                    Alignment = Element.ALIGN_CENTER
                };
                universityLogoCell.AddElement(universityPlaceholder);
            }
        }
        else
        {
            var universityPlaceholder = new Paragraph("شعار الجامعة", new Font(baseFont, 10, Font.BOLD))
            {
                Alignment = Element.ALIGN_CENTER
            };
            universityLogoCell.AddElement(universityPlaceholder);
        }

        // إضافة الخلايا للجدول
        headerTable.AddCell(collegeLogoCell);
        headerTable.AddCell(titleCell);
        headerTable.AddCell(universityLogoCell);

        document.Add(headerTable);

        // خط فاصل
        var lineSeparator = new LineSeparator(1f, 100f, new BaseColor(52, 73, 94), Element.ALIGN_CENTER, -10);
        document.Add(new Chunk(lineSeparator));
        document.Add(Chunk.NEWLINE);
    }

    private void AddFooter(Document document, PdfWriter writer, Font footerFont)
    {
        var lineSeparator = new LineSeparator(1f, 100f, new BaseColor(52, 73, 94), Element.ALIGN_CENTER, -10);
        document.Add(new Chunk(lineSeparator));

        var footerTable = new PdfPTable(3)
        {
            RunDirection = PdfWriter.RUN_DIRECTION_RTL,
            WidthPercentage = 100,
            SpacingBefore = 10f
        };
        footerTable.SetWidths(new float[] { 1f, 1f, 1f });

        var cell1 = new PdfPCell(new Phrase("نظام إدارة بيانات الطلاب", footerFont))
        {
            BorderWidth = 0,
            HorizontalAlignment = Element.ALIGN_RIGHT
        };

        var pageNumber = new PdfPCell(new Phrase($"صفحة {writer.PageNumber}", footerFont))
        {
            BorderWidth = 0,
            HorizontalAlignment = Element.ALIGN_CENTER
        };

        var cell3 = new PdfPCell(new Phrase(DateTime.Now.ToString("dd/MM/yyyy", new CultureInfo("ar-EG")), footerFont))
        {
            BorderWidth = 0,
            HorizontalAlignment = Element.ALIGN_LEFT
        };

        footerTable.AddCell(cell1);
        footerTable.AddCell(pageNumber);
        footerTable.AddCell(cell3);

        document.Add(footerTable);
    }

    private string GetArabicColumnName(string englishName)
    {
        var translations = new Dictionary<string, string>
        {
            { "Id", "الرقم" },
            { "Name", "الاسم" },
            { "NationalId", "الرقم القومي" },
            { "Gender", "الجنس" },
            { "Address", "العنوان" },
            { "Nationality", "الجنسية" },
            { "Email", "البريد الإلكتروني" },
            { "Phone", "الهاتف" },
            { "Semester", "الفصل الدراسي" },
            { "DateOfBirth", "تاريخ الميلاد" },
            { "EnrollmentDate", "تاريخ التسجيل" },
            { "GPA_Average", "المعدل التراكمي" },
            { "High_School_degree", "درجة الثانوية" },
            { "High_School_Section", "قسم الثانوية" },
            { "CreditsCompleted", "الساعات المكتملة" },
            { "DepartmentName", "اسم القسم" },
            { "status", "الحالة" },
            { "StudentLevel", "المستوى الدراسي" },
            { "Credits", "عدد الساعات" },
            { "Description", "الوصف" },
            { "Status", "الحالة" },
            { "MaxSeats", "الحد الاقصى للمقاعد" },
            { "CurrentEnrolledStudents", "عدد الطلاب المسجلين" },
            { "DivisionNames", "الشعبة" },
            { "PreCourseName", "الكورسات السابقة" },
             {"FullName", "الاسم"},
            {"StudentId", "الرقم"},
            {"StudentName", "الاسم"},
            {"CourseName", "الكورس"},
            {"Exam1Grade", "درجة الامتحان الاول"},
            {"Exam2Grade", "درجة الامتحان الثاني"},
            {"Grade", "درجه الامتحان النهائي"},
            {"FinalGrade", "الدرجة النهائية"},
            {"Semster", "الفصل الدراسي"},
            {"Position", "المنصب"}
        };

        return translations.ContainsKey(englishName) ? translations[englishName] : englishName;
    }

    public async Task<byte[]> GenerateProfessorSchedulePdfAsync(IEnumerable<ClassDto> classes, string professorName)
    {
        // التحقق من وجود بيانات
        if (!classes.Any())
        {
            throw new InvalidOperationException("لا توجد محاضرات للعرض");
        }

        using var ms = new MemoryStream();
        var document = new Document(PageSize.A4.Rotate(), 20f, 20f, 20f, 20f);
        var writer = PdfWriter.GetInstance(document, ms);
        writer.RunDirection = PdfWriter.RUN_DIRECTION_RTL;

        document.AddCreator("نظام إدارة الطلاب");
        document.AddAuthor("جامعة الفيوم");
        document.AddSubject($"جدول محاضرات الدكتور {professorName}");
        document.AddTitle($"جدول محاضرات الدكتور {professorName}");
        document.Open();

        // إعداد الخطوط مع تحسين الخط العربي
        BaseFont baseFont = null;
        string[] fontPaths = {
        "C:\\Windows\\Fonts\\arial.ttf",
        "C:\\Windows\\Fonts\\tahoma.ttf",
        "C:\\Windows\\Fonts\\calibri.ttf", // خط أفضل للعربية
        "/usr/share/fonts/truetype/dejavu/DejaVuSans.ttf",
        "/System/Library/Fonts/Arial.ttf" // Mac
    };

        foreach (var path in fontPaths)
        {
            try
            {
                if (File.Exists(path))
                {
                    baseFont = BaseFont.CreateFont(path, BaseFont.IDENTITY_H, BaseFont.EMBEDDED);
                    break;
                }
            }
            catch { continue; }
        }

        if (baseFont == null)
        {
            baseFont = BaseFont.CreateFont(BaseFont.HELVETICA, BaseFont.CP1252, BaseFont.EMBEDDED);
        }

        var titleFont = new Font(baseFont, 16, Font.BOLD);
        var subtitleFont = new Font(baseFont, 12, Font.BOLD);
        var headerFont = new Font(baseFont, 10, Font.BOLD, BaseColor.WHITE);
        var cellFont = new Font(baseFont, 9, Font.NORMAL); // زيادة حجم الخط قليلاً
        var dayFont = new Font(baseFont, 10, Font.BOLD);

        // إضافة الهيدر
        AddScheduleHeader(document, professorName, titleFont, subtitleFont, baseFont);

        // تحويل البيانات إلى جدول أسبوعي
        var scheduleTable = CreateWeeklyScheduleTable(classes, headerFont, cellFont, dayFont, baseFont);
        document.Add(scheduleTable);

        // إضافة معلومات إضافية
        AddScheduleFooterInfo(document, classes, cellFont, baseFont);

        document.Close();
        return ms.ToArray();
    }

    private void AddScheduleHeader(Document document, string professorName, Font titleFont, Font subtitleFont, BaseFont baseFont)
    {
        // جدول الهيدر
        var headerTable = new PdfPTable(3)
        {
            RunDirection = PdfWriter.RUN_DIRECTION_RTL,
            WidthPercentage = 100
        };
        headerTable.SetWidths(new float[] { 1f, 3f, 1f });

        // شعار الكلية
        var collegeLogoCell = new PdfPCell
        {
            BorderWidth = 0,
            HorizontalAlignment = Element.ALIGN_CENTER,
            VerticalAlignment = Element.ALIGN_MIDDLE,
            Padding = 10
        };

        if (File.Exists(_collegeLogoPath))
        {
            try
            {
                var collegeLogoImage = Image.GetInstance(_collegeLogoPath);
                collegeLogoImage.ScaleToFit(60f, 60f);
                collegeLogoImage.Alignment = Element.ALIGN_CENTER;
                collegeLogoCell.AddElement(collegeLogoImage);
            }
            catch
            {
                var collegePlaceholder = new Paragraph("شعار الكلية", new Font(baseFont, 10, Font.BOLD))
                {
                    Alignment = Element.ALIGN_CENTER
                };
                collegeLogoCell.AddElement(collegePlaceholder);
            }
        }

        // النصوص الوسطى
        var titleCell = new PdfPCell
        {
            BorderWidth = 0,
            HorizontalAlignment = Element.ALIGN_CENTER,
            VerticalAlignment = Element.ALIGN_MIDDLE,
            PaddingBottom = 15,
            PaddingTop = 15
        };

        var universityParagraph = new Paragraph("جامعة الفيوم", new Font(baseFont, 18, Font.BOLD))
        {
            Alignment = Element.ALIGN_CENTER,
            SpacingAfter = 3f
        };
        titleCell.AddElement(universityParagraph);

        var collegeParagraph = new Paragraph("كلية العلوم", new Font(baseFont, 14, Font.BOLD))
        {
            Alignment = Element.ALIGN_CENTER,
            SpacingAfter = 3f
        };
        titleCell.AddElement(collegeParagraph);

        var departmentParagraph = new Paragraph("قسم شئون الطلاب", new Font(baseFont, 12, Font.BOLD))
        {
            Alignment = Element.ALIGN_CENTER,
            SpacingAfter = 5f
        };
        titleCell.AddElement(departmentParagraph);

        var semesterParagraph = new Paragraph($"جدول الفصل الدراسي الثاني {DateTime.Now.Year + 1}/{DateTime.Now.Year}", titleFont)
        {
            Alignment = Element.ALIGN_CENTER,
            SpacingAfter = 5f
        };
        titleCell.AddElement(semesterParagraph);

        var professorParagraph = new Paragraph($"أ.د/ {professorName}", subtitleFont)
        {
            Alignment = Element.ALIGN_CENTER
        };
        titleCell.AddElement(professorParagraph);

        // شعار الجامعة
        var universityLogoCell = new PdfPCell
        {
            BorderWidth = 0,
            HorizontalAlignment = Element.ALIGN_CENTER,
            VerticalAlignment = Element.ALIGN_MIDDLE,
            Padding = 10
        };

        if (File.Exists(_universityLogoPath))
        {
            try
            {
                var universityLogoImage = Image.GetInstance(_universityLogoPath);
                universityLogoImage.ScaleToFit(60f, 60f);
                universityLogoImage.Alignment = Element.ALIGN_CENTER;
                universityLogoCell.AddElement(universityLogoImage);
            }
            catch
            {
                var universityPlaceholder = new Paragraph("شعار الجامعة", new Font(baseFont, 10, Font.BOLD))
                {
                    Alignment = Element.ALIGN_CENTER
                };
                universityLogoCell.AddElement(universityPlaceholder);
            }
        }

        headerTable.AddCell(collegeLogoCell);
        headerTable.AddCell(titleCell);
        headerTable.AddCell(universityLogoCell);

        document.Add(headerTable);
        document.Add(Chunk.NEWLINE);
    }

    private PdfPTable CreateWeeklyScheduleTable(IEnumerable<ClassDto> classes, Font headerFont, Font cellFont, Font dayFont, BaseFont baseFont)
    {
        // إنشاء قائمة الأوقات بناءً على المحاضرات الموجودة
        var timeSlots = GenerateTimeSlots(classes);

        // أيام الأسبوع
        var daysOfWeek = new List<string>
    {
        "Saturday", "Sunday", "Monday", "Tuesday", "Wednesday", "Thursday"
    };

        var dayNamesArabic = new Dictionary<string, string>
    {
        { "Saturday", "السبت" },
        { "Sunday", "الأحد" },
        { "Monday", "الاثنين" },
        { "Tuesday", "الثلاثاء" },
        { "Wednesday", "الأربعاء" },
        { "Thursday", "الخميس" }
    };

        // إنشاء الجدول
        var table = new PdfPTable(timeSlots.Count + 1)
        {
            RunDirection = PdfWriter.RUN_DIRECTION_RTL,
            WidthPercentage = 100,
            SpacingBefore = 10f,
            SpacingAfter = 10f
        };

        // تحديد عرض الأعمدة بشكل متساوي أكثر
        var widths = new List<float> { 1.5f }; // عمود الأيام أعرض قليلاً
        widths.AddRange(Enumerable.Repeat(1f, timeSlots.Count));
        table.SetWidths(widths.ToArray());

        // يطابق --primary-color: #003366
        var headerBlueColor = new BaseColor(91, 155, 213);
        var lightBlueColor = new BaseColor(217, 237, 247);
        // أزرق فاتح للأيام
        var whiteColor = BaseColor.WHITE;

        // صف واحد للـ Time
        var timeHeaderCell = new PdfPCell(new Phrase("الوقت", new Font(baseFont, 10, Font.BOLD, BaseColor.WHITE)))
        {
            RunDirection = PdfWriter.RUN_DIRECTION_RTL,
            HorizontalAlignment = Element.ALIGN_CENTER,
            VerticalAlignment = Element.ALIGN_MIDDLE,
            BackgroundColor = headerBlueColor,
            BorderColor = BaseColor.BLACK,
            BorderWidth = 0.5f,
            Padding = 8,
            MinimumHeight = 30f
        };
        table.AddCell(timeHeaderCell);

        // إضافة خلايا الأوقات
        foreach (var timeSlot in timeSlots)
        {
            var timeCell = new PdfPCell(new Phrase(timeSlot.DisplayText, new Font(baseFont, 9, Font.BOLD, BaseColor.WHITE)))
            {
                RunDirection = PdfWriter.RUN_DIRECTION_RTL,
                HorizontalAlignment = Element.ALIGN_CENTER,
                VerticalAlignment = Element.ALIGN_MIDDLE,
                BackgroundColor = headerBlueColor,
                BorderColor = BaseColor.BLACK,
                BorderWidth = 0.5f,
                Padding = 8,
                MinimumHeight = 30f
            };
            table.AddCell(timeCell);
        }

        // إضافة صفوف الأيام مع معالجة دمج الخلايا للمحاضرات الطويلة
        foreach (var day in daysOfWeek)
        {
            // خلية اليوم
            var dayCell = new PdfPCell(new Phrase(dayNamesArabic[day], new Font(baseFont, 11, Font.BOLD)))
            {
                RunDirection = PdfWriter.RUN_DIRECTION_RTL,
                HorizontalAlignment = Element.ALIGN_CENTER,
                VerticalAlignment = Element.ALIGN_MIDDLE,
                BackgroundColor = lightBlueColor,
                BorderColor = BaseColor.BLACK,
                BorderWidth = 0.5f,
                Padding = 10,
                MinimumHeight = 70f
            };
            table.AddCell(dayCell);

            // تتبع الخلايا المدموجة
            var mergedCells = new HashSet<int>();

            // خلايا الأوقات لهذا اليوم
            for (int timeIndex = 0; timeIndex < timeSlots.Count; timeIndex++)
            {
                if (mergedCells.Contains(timeIndex))
                {
                    continue; // تخطي الخلايا المدموجة
                }

                var timeSlot = timeSlots[timeIndex];
                var classesForSlot = GetClassesForDayAndTimeSlot(classes, day, timeSlot);

                var cell = new PdfPCell()
                {
                    RunDirection = PdfWriter.RUN_DIRECTION_RTL,
                    HorizontalAlignment = Element.ALIGN_CENTER,
                    VerticalAlignment = Element.ALIGN_MIDDLE,
                    BorderColor = BaseColor.BLACK,
                    BorderWidth = 0.5f,
                    Padding = 5,
                    MinimumHeight = 70f
                };

                if (classesForSlot.Any())
                {
                    var classInfo = classesForSlot.First();

                    // حساب مدة المحاضرة بالساعات
                    var duration = classInfo.EndTime - classInfo.StartTime;
                    int durationHours = (int)Math.Round(duration.TotalHours);

                    // إذا كانت المحاضرة أكثر من ساعة، دمج الخلايا
                    if (durationHours > 1)
                    {
                        // حساب عدد الخلايا التي يجب دمجها
                        int cellsToMerge = 0;
                        for (int i = timeIndex; i < timeSlots.Count && cellsToMerge < durationHours; i++)
                        {
                            if (timeSlots[i].Hour < classInfo.EndTime.Hours)
                            {
                                cellsToMerge++;
                            }
                            else
                            {
                                break;
                            }
                        }

                        if (cellsToMerge > 1)
                        {
                            cell.Colspan = cellsToMerge;

                            // تسجيل الخلايا المدموجة
                            for (int i = 1; i < cellsToMerge; i++)
                            {
                                mergedCells.Add(timeIndex + i);
                            }
                        }
                    }

                    // إضافة المحاضرة للخلية مع تنسيق أفضل
                    var paragraph = new Paragraph();
                    paragraph.Alignment = Element.ALIGN_CENTER;

                    // اسم المقرر
                    if (!string.IsNullOrEmpty(classInfo.CourseName))
                    {
                        var courseName = classInfo.CourseName.Length > 25
                            ? classInfo.CourseName.Substring(0, 22) + "..."
                            : classInfo.CourseName;

                        paragraph.Add(new Phrase(courseName, new Font(baseFont, 9, Font.BOLD)));
                        paragraph.Add(Chunk.NEWLINE);
                    }

                    // المكان
                    if (!string.IsNullOrEmpty(classInfo.Location))
                    {
                        paragraph.Add(new Phrase(classInfo.Location, new Font(baseFont, 8, Font.NORMAL)));
                        paragraph.Add(Chunk.NEWLINE);
                    }

                    // الشعبة
                    if (!string.IsNullOrEmpty(classInfo.DivisionName) &&
                        classInfo.DivisionName != "عام" &&
                        classInfo.DivisionName != "لا يوجد شُعب مسجّلة")
                    {
                        paragraph.Add(new Phrase(classInfo.DivisionName, new Font(baseFont, 7, Font.ITALIC)));
                    }

                    cell.AddElement(paragraph);
                    cell.BackgroundColor = whiteColor;
                }
                else
                {
                    // خلية فارغة
                    cell.BackgroundColor = whiteColor;
                }

                table.AddCell(cell);
            }
        }

        return table;
    }

    // كلاس مساعد لحفظ معلومات الوقت
    public class TimeSlotInfo
    {
        public int Hour { get; set; }
        public string DisplayText { get; set; }
    }

    private List<TimeSlotInfo> GenerateTimeSlots(IEnumerable<ClassDto> classes)
    {
        // جدول أوقات ثابت من 8 صباحاً إلى 6 مساءً (الساعة 18)
        var standardHours = new List<int> { 8, 9, 10, 11, 12, 13, 14, 15, 16, 17, 18 };

        // ترتيب الأوقات وإنشاء قائمة TimeSlotInfo
        var timeSlots = standardHours.Select(hour => new TimeSlotInfo
        {
            Hour = hour,
            DisplayText = FormatHour(hour)
        }).ToList();

        return timeSlots;
    }

    private string FormatHour(int hour)
    {
        switch (hour)
        {
            case 8: return "8:00";
            case 9: return "9:00";
            case 10: return "10:00";
            case 11: return "11:00";
            case 12: return "12:00 PM";
            case 13: return "1:00 PM";
            case 14: return "2:00 PM";
            case 15: return "3:00 PM";
            case 16: return "4:00 PM";
            case 17: return "5:00 PM";
            case 18: return "6:00 PM";
            default:
                if (hour < 12) return $"{hour}:00";
                if (hour == 12) return "12:00 PM";
                return $"{hour - 12}:00 PM";
        }
    }

    private List<ClassDto> GetClassesForDayAndTimeSlot(IEnumerable<ClassDto> classes, string day, TimeSlotInfo timeSlot)
    {
        try
        {
            // تحويل اسم اليوم من الإنجليزية إلى العربية للمقارنة
            var dayMapping = new Dictionary<string, string>
        {
            { "Saturday", "السبت" },
            { "Sunday", "الأحد" },
            { "Monday", "الاثنين" },
            { "Tuesday", "الثلاثاء" },
            { "Wednesday", "الأربعاء" },
            { "Thursday", "الخميس" }
        };

            if (!dayMapping.ContainsKey(day))
            {
                return new List<ClassDto>();
            }

            string expectedDayArabic = dayMapping[day];

            // البحث عن المحاضرات التي تبدأ في هذا الوقت تحديداً
            var classesForSlot = classes.Where(c =>
            {
                // مقارنة اسم اليوم
                string classDay = c.Day?.Trim();
                bool dayMatch = string.Equals(classDay, expectedDayArabic, StringComparison.OrdinalIgnoreCase);

                // التحقق من أن المحاضرة تبدأ في هذا الوقت تحديداً
                bool timeMatch = c.StartTime.Hours == timeSlot.Hour;

                return dayMatch && timeMatch;
            }).ToList();

            return classesForSlot;
        }
        catch (Exception ex)
        {
            // يمكنك إضافة logging هنا
            Console.WriteLine($"خطأ في GetClassesForDayAndTimeSlot: {ex.Message}");
            return new List<ClassDto>();
        }
    }
    private List<ClassDto> GetClassesForDayAndTime(IEnumerable<ClassDto> classes, string day, string timeSlot)
    {
        try
        {
            if (!int.TryParse(timeSlot.Split(':')[0], out int timeHour))
            {
                return new List<ClassDto>();
            }

            var slotTime = new TimeSpan(timeHour, 0, 0);

            // تحويل اسم اليوم من الإنجليزية إلى العربية للمقارنة
            var dayMapping = new Dictionary<string, string>
        {
            { "Saturday", "السبت" },
            { "Sunday", "الأحد" },
            { "Monday", "الاثنين" },
            { "Tuesday", "الثلاثاء" },
            { "Wednesday", "الأربعاء" },
            { "Thursday", "الخميس" }
        };

            if (!dayMapping.ContainsKey(day))
            {
                return new List<ClassDto>();
            }

            string expectedDayArabic = dayMapping[day];

            // البحث عن المحاضرات التي تبدأ في هذا الوقت تحديداً
            var classesForSlot = classes.Where(c =>
            {
                // مقارنة اسم اليوم
                string classDay = c.Day?.Trim();
                bool dayMatch = string.Equals(classDay, expectedDayArabic, StringComparison.OrdinalIgnoreCase);

                // التحقق من أن المحاضرة تبدأ في هذا الوقت تحديداً
                bool timeMatch = c.StartTime.Hours == timeHour;

                return dayMatch && timeMatch;
            }).ToList();

            return classesForSlot;
        }
        catch (Exception)
        {
            return new List<ClassDto>();
        }
    }

    private void AddScheduleFooterInfo(Document document, IEnumerable classes, Font cellFont, BaseFont baseFont)
    {
        document.Add(Chunk.NEWLINE);

        // تقليل المسافة قبل التوقيعات
        var signatureTable = new PdfPTable(3)
        {
            RunDirection = PdfWriter.RUN_DIRECTION_RTL,
            WidthPercentage = 100,
            SpacingBefore = 15f // تقليل المسافة
        };
        signatureTable.SetWidths(new float[] { 1f, 1f, 1f });

        // تحسين تنسيق خلايا التوقيع مع تقليل المسافات
        var deanCell = new PdfPCell()
        {
            BorderWidth = 0,
            HorizontalAlignment = Element.ALIGN_CENTER,
            VerticalAlignment = Element.ALIGN_MIDDLE,
            Padding = 4, // تقليل الحشو
            MinimumHeight = 20f, // تقليل الارتفاع
            RunDirection = PdfWriter.RUN_DIRECTION_RTL // إضافة اتجاه RTL للخلية
        };

        var deanParagraph = new Paragraph();
        deanParagraph.Add(new Phrase("عميد الكلية", new Font(baseFont, 11, Font.BOLD)));
        deanParagraph.Add(Chunk.NEWLINE);
        deanParagraph.Add(new Phrase("_________________", new Font(baseFont, 10, Font.NORMAL)));
        deanParagraph.Add(Chunk.NEWLINE);
        deanParagraph.Add(new Phrase("أ.د/ سميه السيد جوده", new Font(baseFont, 10, Font.NORMAL)));
        deanParagraph.Alignment = Element.ALIGN_CENTER;
        deanParagraph.SpacingBefore = 0f;
        deanParagraph.SpacingAfter = 0f;
        deanCell.AddElement(deanParagraph);

        var coordinatorCell = new PdfPCell()
        {
            BorderWidth = 0,
            HorizontalAlignment = Element.ALIGN_CENTER,
            VerticalAlignment = Element.ALIGN_MIDDLE,
            Padding = 4,
            MinimumHeight = 20f,
            RunDirection = PdfWriter.RUN_DIRECTION_RTL // إضافة اتجاه RTL للخلية
        };

        var coordinatorParagraph = new Paragraph();
        coordinatorParagraph.Add(new Phrase("وكيل الكلية لشئون التعليم والطلاب", new Font(baseFont, 11, Font.BOLD)));
        coordinatorParagraph.Add(Chunk.NEWLINE);
        coordinatorParagraph.Add(new Phrase("_________________", new Font(baseFont, 10, Font.NORMAL)));
        coordinatorParagraph.Add(Chunk.NEWLINE);
        coordinatorParagraph.Add(new Phrase("أ.د/ احمد روبي شافعي", new Font(baseFont, 10, Font.NORMAL)));
        coordinatorParagraph.Alignment = Element.ALIGN_CENTER;
        coordinatorParagraph.SpacingBefore = 0f;
        coordinatorParagraph.SpacingAfter = 0f;
        coordinatorCell.AddElement(coordinatorParagraph);

        var headCell = new PdfPCell()
        {
            BorderWidth = 0,
            HorizontalAlignment = Element.ALIGN_CENTER,
            VerticalAlignment = Element.ALIGN_MIDDLE,
            Padding = 4,
            MinimumHeight = 20f,
            RunDirection = PdfWriter.RUN_DIRECTION_RTL // إضافة اتجاه RTL للخلية
        };

        var headParagraph = new Paragraph();
        headParagraph.Add(new Phrase("رئيس قسم شئون الطلاب", new Font(baseFont, 11, Font.BOLD)));
        headParagraph.Add(Chunk.NEWLINE);
        headParagraph.Add(new Phrase("_________________", new Font(baseFont, 10, Font.NORMAL)));
        headParagraph.Alignment = Element.ALIGN_CENTER;
        headParagraph.SpacingBefore = 0f;
        headParagraph.SpacingAfter = 0f;
        headCell.AddElement(headParagraph);

        signatureTable.AddCell(deanCell);
        signatureTable.AddCell(coordinatorCell);
        signatureTable.AddCell(headCell);
        document.Add(signatureTable);

        // الحل الصحيح: استخدام جدول منفصل للنص السفلي
        var footerTable = new PdfPTable(1)
        {
            RunDirection = PdfWriter.RUN_DIRECTION_RTL,
            WidthPercentage = 100,
            SpacingBefore = 10f
        };

        var footerCell = new PdfPCell()
        {
            BorderWidth = 0,
            HorizontalAlignment = Element.ALIGN_CENTER,
            VerticalAlignment = Element.ALIGN_MIDDLE,
            RunDirection = PdfWriter.RUN_DIRECTION_RTL,
            Padding = 0
        };

        var footerParagraph = new Paragraph("مكتب وكيل الكلية لشئون التعليم والطلاب", new Font(baseFont, 9, Font.NORMAL))
        {
            Alignment = Element.ALIGN_CENTER
        };

        footerCell.AddElement(footerParagraph);
        footerTable.AddCell(footerCell);
        document.Add(footerTable);
    }

    public async Task<byte[]> GenerateStudentSchedulePdfAsync(IEnumerable<ClassDto> classes, string StudentName)
    {
        // التحقق من وجود بيانات
        if (!classes.Any())
        {
            throw new InvalidOperationException("لا توجد محاضرات للعرض");
        }

        using var ms = new MemoryStream();
        var document = new Document(PageSize.A4.Rotate(), 20f, 20f, 20f, 20f);
        var writer = PdfWriter.GetInstance(document, ms);
        writer.RunDirection = PdfWriter.RUN_DIRECTION_RTL;

        document.AddCreator("نظام إدارة الطلاب");
        document.AddAuthor("جامعة الفيوم");
        document.AddSubject($"جدول محاضرات الطالب {StudentName}");
        document.AddTitle($"جدول محاضرات الطالب {StudentName}");
        document.Open();

        // إعداد الخطوط مع تحسين الخط العربي
        BaseFont baseFont = null;
        string[] fontPaths = {
        "C:\\Windows\\Fonts\\arial.ttf",
        "C:\\Windows\\Fonts\\tahoma.ttf",
        "C:\\Windows\\Fonts\\calibri.ttf", // خط أفضل للعربية
        "/usr/share/fonts/truetype/dejavu/DejaVuSans.ttf",
        "/System/Library/Fonts/Arial.ttf" // Mac
    };

        foreach (var path in fontPaths)
        {
            try
            {
                if (File.Exists(path))
                {
                    baseFont = BaseFont.CreateFont(path, BaseFont.IDENTITY_H, BaseFont.EMBEDDED);
                    break;
                }
            }
            catch { continue; }
        }

        if (baseFont == null)
        {
            baseFont = BaseFont.CreateFont(BaseFont.HELVETICA, BaseFont.CP1252, BaseFont.EMBEDDED);
        }

        var titleFont = new Font(baseFont, 16, Font.BOLD);
        var subtitleFont = new Font(baseFont, 12, Font.BOLD);
        var headerFont = new Font(baseFont, 10, Font.BOLD, BaseColor.WHITE);
        var cellFont = new Font(baseFont, 9, Font.NORMAL); // زيادة حجم الخط قليلاً
        var dayFont = new Font(baseFont, 10, Font.BOLD);

        // إضافة الهيدر
        AddScheduleStudentHeader(document, StudentName, titleFont, subtitleFont, baseFont);

        // تحويل البيانات إلى جدول أسبوعي
        var scheduleTable = CreateWeeklyScheduleTable(classes, headerFont, cellFont, dayFont, baseFont);
        document.Add(scheduleTable);

        // إضافة معلومات إضافية
        AddScheduleFooterInfo(document, classes, cellFont, baseFont);

        document.Close();
        return ms.ToArray();
    }

    private void AddScheduleStudentHeader(Document document, string StudentName, Font titleFont, Font subtitleFont, BaseFont baseFont)
    {
        // جدول الهيدر
        var headerTable = new PdfPTable(3)
        {
            RunDirection = PdfWriter.RUN_DIRECTION_RTL,
            WidthPercentage = 100
        };
        headerTable.SetWidths(new float[] { 1f, 3f, 1f });

        // شعار الكلية
        var collegeLogoCell = new PdfPCell
        {
            BorderWidth = 0,
            HorizontalAlignment = Element.ALIGN_CENTER,
            VerticalAlignment = Element.ALIGN_MIDDLE,
            Padding = 10
        };

        if (File.Exists(_collegeLogoPath))
        {
            try
            {
                var collegeLogoImage = Image.GetInstance(_collegeLogoPath);
                collegeLogoImage.ScaleToFit(60f, 60f);
                collegeLogoImage.Alignment = Element.ALIGN_CENTER;
                collegeLogoCell.AddElement(collegeLogoImage);
            }
            catch
            {
                var collegePlaceholder = new Paragraph("شعار الكلية", new Font(baseFont, 10, Font.BOLD))
                {
                    Alignment = Element.ALIGN_CENTER
                };
                collegeLogoCell.AddElement(collegePlaceholder);
            }
        }

        // النصوص الوسطى
        var titleCell = new PdfPCell
        {
            BorderWidth = 0,
            HorizontalAlignment = Element.ALIGN_CENTER,
            VerticalAlignment = Element.ALIGN_MIDDLE,
            PaddingBottom = 15,
            PaddingTop = 15
        };

        var universityParagraph = new Paragraph("جامعة الفيوم", new Font(baseFont, 18, Font.BOLD))
        {
            Alignment = Element.ALIGN_CENTER,
            SpacingAfter = 3f
        };
        titleCell.AddElement(universityParagraph);

        var collegeParagraph = new Paragraph("كلية العلوم", new Font(baseFont, 14, Font.BOLD))
        {
            Alignment = Element.ALIGN_CENTER,
            SpacingAfter = 3f
        };
        titleCell.AddElement(collegeParagraph);

        var departmentParagraph = new Paragraph("قسم شئون الطلاب", new Font(baseFont, 12, Font.BOLD))
        {
            Alignment = Element.ALIGN_CENTER,
            SpacingAfter = 5f
        };
        titleCell.AddElement(departmentParagraph);

        var semesterParagraph = new Paragraph($"جدول الفصل الدراسي الثاني {DateTime.Now.Year + 1}/{DateTime.Now.Year}", titleFont)
        {
            Alignment = Element.ALIGN_CENTER,
            SpacingAfter = 5f
        };
        titleCell.AddElement(semesterParagraph);

        var professorParagraph = new Paragraph($"ط/ {StudentName}", subtitleFont)
        {
            Alignment = Element.ALIGN_CENTER
        };
        titleCell.AddElement(professorParagraph);

        // شعار الجامعة
        var universityLogoCell = new PdfPCell
        {
            BorderWidth = 0,
            HorizontalAlignment = Element.ALIGN_CENTER,
            VerticalAlignment = Element.ALIGN_MIDDLE,
            Padding = 10
        };

        if (File.Exists(_universityLogoPath))
        {
            try
            {
                var universityLogoImage = Image.GetInstance(_universityLogoPath);
                universityLogoImage.ScaleToFit(60f, 60f);
                universityLogoImage.Alignment = Element.ALIGN_CENTER;
                universityLogoCell.AddElement(universityLogoImage);
            }
            catch
            {
                var universityPlaceholder = new Paragraph("شعار الجامعة", new Font(baseFont, 10, Font.BOLD))
                {
                    Alignment = Element.ALIGN_CENTER
                };
                universityLogoCell.AddElement(universityPlaceholder);
            }
        }

        headerTable.AddCell(collegeLogoCell);
        headerTable.AddCell(titleCell);
        headerTable.AddCell(universityLogoCell);

        document.Add(headerTable);
        document.Add(Chunk.NEWLINE);
    }



    public async Task<byte[]> GenerateClassesPdfAsync(IEnumerable<ClassDto> classes, string? filterInfo = null)
    {
        // التحقق من وجود بيانات
        if (!classes.Any())
        {
            throw new InvalidOperationException("لا توجد محاضرات للعرض");
        }

        using var ms = new MemoryStream();
        var document = new Document(PageSize.A4.Rotate(), 20f, 20f, 20f, 20f);
        var writer = PdfWriter.GetInstance(document, ms);
        writer.RunDirection = PdfWriter.RUN_DIRECTION_RTL;

        document.AddCreator("نظام إدارة الطلاب");
        document.AddAuthor("جامعة الفيوم");
        document.AddSubject("جدول المحاضرات العام");
        document.AddTitle("جدول المحاضرات العام");
        document.Open();

        // إعداد الخطوط
        BaseFont baseFont = GetArabicFont();
        var titleFont = new Font(baseFont, 16, Font.BOLD);
        var subtitleFont = new Font(baseFont, 12, Font.BOLD);
        var headerFont = new Font(baseFont, 10, Font.BOLD, BaseColor.WHITE);
        var cellFont = new Font(baseFont, 9, Font.NORMAL);
        var dayFont = new Font(baseFont, 10, Font.BOLD);
        var levelTitleFont = new Font(baseFont, 14, Font.BOLD, new BaseColor(0, 51, 102)); // أزرق داكن

        // إضافة الهيدر العام
        AddGeneralScheduleHeader(document, titleFont, subtitleFont, baseFont, filterInfo);

        // تجميع المحاضرات حسب المستوى (الفرقة)
        var classesGroupedByLevel = classes
            .GroupBy(c => c.Level ?? "غير محدد")
            .OrderBy(g => GetLevelOrder(g.Key))
            .ToList();

        bool isFirstLevel = true;
        foreach (var levelGroup in classesGroupedByLevel)
        {
            // إضافة مسافة بين الجداول (ما عدا الأول)
            if (!isFirstLevel)
            {
                document.NewPage(); // صفحة جديدة لكل فرقة
            }
            isFirstLevel = false;

            // عنوان الفرقة
            AddLevelTitle(document, levelGroup.Key, levelTitleFont, baseFont);

            // إحصائيات سريعة للفرقة
            AddLevelStatistics(document, levelGroup, baseFont);

            // إنشاء جدول المحاضرات للفرقة
            var levelScheduleTable = CreateWeeklyScheduleTable(levelGroup, headerFont, cellFont, dayFont, baseFont);
            document.Add(levelScheduleTable);

            // إضافة قائمة بأسماء الأساتذة والمواد للفرقة
            AddLevelCoursesAndProfessors(document, levelGroup, baseFont);
        }

        // إضافة الفوتر العام
        AddScheduleFooterInfo(document, classes, cellFont, baseFont);

        document.Close();
        return ms.ToArray();
    }

    private void AddGeneralScheduleHeader(Document document, Font titleFont, Font subtitleFont, BaseFont baseFont, string? filterInfo)
    {
        // جدول الهيدر
        var headerTable = new PdfPTable(3)
        {
            RunDirection = PdfWriter.RUN_DIRECTION_RTL,
            WidthPercentage = 100
        };
        headerTable.SetWidths(new float[] { 1f, 3f, 1f });

        // شعار الكلية
        var collegeLogoCell = new PdfPCell
        {
            BorderWidth = 0,
            HorizontalAlignment = Element.ALIGN_CENTER,
            VerticalAlignment = Element.ALIGN_MIDDLE,
            Padding = 10
        };

        if (File.Exists(_collegeLogoPath))
        {
            try
            {
                var collegeLogoImage = Image.GetInstance(_collegeLogoPath);
                collegeLogoImage.ScaleToFit(60f, 60f);
                collegeLogoImage.Alignment = Element.ALIGN_CENTER;
                collegeLogoCell.AddElement(collegeLogoImage);
            }
            catch
            {
                var collegePlaceholder = new Paragraph("شعار الكلية", new Font(baseFont, 10, Font.BOLD))
                {
                    Alignment = Element.ALIGN_CENTER
                };
                collegeLogoCell.AddElement(collegePlaceholder);
            }
        }

        // النصوص الوسطى
        var titleCell = new PdfPCell
        {
            BorderWidth = 0,
            HorizontalAlignment = Element.ALIGN_CENTER,
            VerticalAlignment = Element.ALIGN_MIDDLE,
            PaddingBottom = 15,
            PaddingTop = 15
        };

        var universityParagraph = new Paragraph("جامعة الفيوم", new Font(baseFont, 18, Font.BOLD))
        {
            Alignment = Element.ALIGN_CENTER,
            SpacingAfter = 3f
        };
        titleCell.AddElement(universityParagraph);

        var collegeParagraph = new Paragraph("كلية العلوم", new Font(baseFont, 14, Font.BOLD))
        {
            Alignment = Element.ALIGN_CENTER,
            SpacingAfter = 3f
        };
        titleCell.AddElement(collegeParagraph);

        var departmentParagraph = new Paragraph("قسم شئون الطلاب", new Font(baseFont, 12, Font.BOLD))
        {
            Alignment = Element.ALIGN_CENTER,
            SpacingAfter = 5f
        };
        titleCell.AddElement(departmentParagraph);

        var semesterParagraph = new Paragraph($"الجدول العام للفصل الدراسي الثاني {DateTime.Now.Year + 1}/{DateTime.Now.Year}", titleFont)
        {
            Alignment = Element.ALIGN_CENTER,
            SpacingAfter = 5f
        };
        titleCell.AddElement(semesterParagraph);

        // إضافة معلومات الفلتر إذا كانت موجودة
        if (!string.IsNullOrEmpty(filterInfo))
        {
            var filterParagraph = new Paragraph($"({filterInfo})", subtitleFont)
            {
                Alignment = Element.ALIGN_CENTER
            };
            titleCell.AddElement(filterParagraph);
        }

        // شعار الجامعة
        var universityLogoCell = new PdfPCell
        {
            BorderWidth = 0,
            HorizontalAlignment = Element.ALIGN_CENTER,
            VerticalAlignment = Element.ALIGN_MIDDLE,
            Padding = 10
        };

        if (File.Exists(_universityLogoPath))
        {
            try
            {
                var universityLogoImage = Image.GetInstance(_universityLogoPath);
                universityLogoImage.ScaleToFit(60f, 60f);
                universityLogoImage.Alignment = Element.ALIGN_CENTER;
                universityLogoCell.AddElement(universityLogoImage);
            }
            catch
            {
                var universityPlaceholder = new Paragraph("شعار الجامعة", new Font(baseFont, 10, Font.BOLD))
                {
                    Alignment = Element.ALIGN_CENTER
                };
                universityLogoCell.AddElement(universityPlaceholder);
            }
        }

        headerTable.AddCell(collegeLogoCell);
        headerTable.AddCell(titleCell);
        headerTable.AddCell(universityLogoCell);

        document.Add(headerTable);
        document.Add(Chunk.NEWLINE);
    }

    private void AddLevelTitle(Document document, string levelName, Font levelTitleFont, BaseFont baseFont)
    {
        // إطار ملون لعنوان الفرقة
        var levelTitleTable = new PdfPTable(1)
        {
            RunDirection = PdfWriter.RUN_DIRECTION_RTL,
            WidthPercentage = 100,
            SpacingBefore = 15f,
            SpacingAfter = 10f
        };

        var levelTitleCell = new PdfPCell(new Phrase($"سنة {levelName}", levelTitleFont))
        {
            RunDirection = PdfWriter.RUN_DIRECTION_RTL,
            HorizontalAlignment = Element.ALIGN_CENTER,
            VerticalAlignment = Element.ALIGN_MIDDLE,
            BackgroundColor = new BaseColor(217, 237, 247), // أزرق فاتح
            BorderColor = new BaseColor(0, 51, 102), // أزرق داكن
            BorderWidth = 1f,
            Padding = 10,
            MinimumHeight = 35f
        };

        levelTitleTable.AddCell(levelTitleCell);
        document.Add(levelTitleTable);
    }

    private void AddLevelStatistics(Document document, IGrouping<string, ClassDto> levelGroup, BaseFont baseFont)
    {
        var statsTable = new PdfPTable(4)
        {
            RunDirection = PdfWriter.RUN_DIRECTION_RTL,
            WidthPercentage = 100,
            SpacingAfter = 10f
        };
        statsTable.SetWidths(new float[] { 1f, 1f, 1f, 1f });

        var totalClasses = levelGroup.Count();
        var totalCourses = levelGroup.Select(c => c.CourseName).Distinct().Count();
        var totalProfessors = levelGroup.Select(c => c.ProfessorName).Distinct().Count();
        var totalDivisions = levelGroup.SelectMany(c => c.DivisionName?.Split('،') ?? new[] { "عام" }).Distinct().Count();

        var headerColor = new BaseColor(91, 155, 213);
        var statsFont = new Font(baseFont, 9, Font.BOLD, BaseColor.WHITE);

        // Headers
        statsTable.AddCell(CreateStatsCell("إجمالي المحاضرات", statsFont, headerColor));
        statsTable.AddCell(CreateStatsCell("عدد المواد", statsFont, headerColor));
        statsTable.AddCell(CreateStatsCell("عدد الأساتذة", statsFont, headerColor));
        statsTable.AddCell(CreateStatsCell("عدد الشعب", statsFont, headerColor));

        // Values
        var valueFont = new Font(baseFont, 9, Font.NORMAL);
        statsTable.AddCell(CreateStatsCell(totalClasses.ToString(), valueFont, BaseColor.WHITE));
        statsTable.AddCell(CreateStatsCell(totalCourses.ToString(), valueFont, BaseColor.WHITE));
        statsTable.AddCell(CreateStatsCell(totalProfessors.ToString(), valueFont, BaseColor.WHITE));
        statsTable.AddCell(CreateStatsCell(totalDivisions.ToString(), valueFont, BaseColor.WHITE));

        document.Add(statsTable);
    }

    private PdfPCell CreateStatsCell(string text, Font font, BaseColor bgColor)
    {
        return new PdfPCell(new Phrase(text, font))
        {
            RunDirection = PdfWriter.RUN_DIRECTION_RTL,
            HorizontalAlignment = Element.ALIGN_CENTER,
            VerticalAlignment = Element.ALIGN_MIDDLE,
            BackgroundColor = bgColor,
            BorderColor = BaseColor.BLACK,
            BorderWidth = 0.5f,
            Padding = 5,
            MinimumHeight = 25f
        };
    }

    private void AddLevelCoursesAndProfessors(Document document, IGrouping<string, ClassDto> levelGroup, BaseFont baseFont)
    {
        document.Add(new Paragraph(" ", new Font(baseFont, 6))); // مسافة صغيرة

        var infoTable = new PdfPTable(2)
        {
            RunDirection = PdfWriter.RUN_DIRECTION_RTL,
            WidthPercentage = 100,
            SpacingBefore = 10f,
            SpacingAfter = 15f
        };
        infoTable.SetWidths(new float[] { 1f, 1f });

        // قائمة المواد
        var coursesCell = new PdfPCell()
        {
            BorderWidth = 0.5f,
            BorderColor = BaseColor.GRAY,
            HorizontalAlignment = Element.ALIGN_RIGHT,
            VerticalAlignment = Element.ALIGN_TOP,
            Padding = 8,
            RunDirection = PdfWriter.RUN_DIRECTION_RTL
        };

        var coursesParagraph = new Paragraph();
        coursesParagraph.Add(new Phrase("المواد الدراسية:", new Font(baseFont, 10, Font.BOLD)));
        coursesParagraph.Add(Chunk.NEWLINE);

        var courses = levelGroup.Select(c => c.CourseName).Distinct().OrderBy(c => c).ToList();
        for (int i = 0; i < courses.Count; i++)
        {
            coursesParagraph.Add(new Phrase($"{i + 1}. {courses[i]}", new Font(baseFont, 8, Font.NORMAL)));
            if (i < courses.Count - 1) coursesParagraph.Add(Chunk.NEWLINE);
        }

        coursesParagraph.Alignment = Element.ALIGN_RIGHT;
        coursesCell.AddElement(coursesParagraph);

        // قائمة الأساتذة
        var professorsCell = new PdfPCell()
        {
            BorderWidth = 0.5f,
            BorderColor = BaseColor.GRAY,
            HorizontalAlignment = Element.ALIGN_RIGHT,
            VerticalAlignment = Element.ALIGN_TOP,
            Padding = 8,
            RunDirection = PdfWriter.RUN_DIRECTION_RTL
        };

        var professorsParagraph = new Paragraph();
        professorsParagraph.Add(new Phrase("أعضاء هيئة التدريس:", new Font(baseFont, 10, Font.BOLD)));
        professorsParagraph.Add(Chunk.NEWLINE);

        var professors = levelGroup.Select(c => c.ProfessorName).Distinct().OrderBy(p => p).ToList();
        for (int i = 0; i < professors.Count; i++)
        {
            professorsParagraph.Add(new Phrase($"{i + 1}. {professors[i]}", new Font(baseFont, 8, Font.NORMAL)));
            if (i < professors.Count - 1) professorsParagraph.Add(Chunk.NEWLINE);
        }

        professorsParagraph.Alignment = Element.ALIGN_RIGHT;
        professorsCell.AddElement(professorsParagraph);

        infoTable.AddCell(coursesCell);
        infoTable.AddCell(professorsCell);
        document.Add(infoTable);
    }

    private int GetLevelOrder(string levelName)
    {
        // ترتيب الفرق بالترتيب الصحيح
        return levelName switch
        {
            "الأولى" => 1,
            "الثانية" => 2,
            "الثالثة" => 3,
            "الرابعة" => 4,
            "غير محدد" => 999,
            _ => 999
        };
    }

    private BaseFont GetArabicFont()
    {
        BaseFont baseFont = null;
        string[] fontPaths = {
        "C:\\Windows\\Fonts\\arial.ttf",
        "C:\\Windows\\Fonts\\tahoma.ttf",
        "C:\\Windows\\Fonts\\calibri.ttf",
        "/usr/share/fonts/truetype/dejavu/DejaVuSans.ttf",
        "/System/Library/Fonts/Arial.ttf"
    };

        foreach (var path in fontPaths)
        {
            try
            {
                if (File.Exists(path))
                {
                    baseFont = BaseFont.CreateFont(path, BaseFont.IDENTITY_H, BaseFont.EMBEDDED);
                    break;
                }
            }
            catch { continue; }
        }

        if (baseFont == null)
        {
            baseFont = BaseFont.CreateFont(BaseFont.HELVETICA, BaseFont.CP1252, BaseFont.EMBEDDED);
        }

        return baseFont;
    }

    public async Task<byte[]> GenerateAdminClassesPdfAsync(IEnumerable<ClassDto> classes, string? filterInfo = null)
    {
        // التحقق من وجود بيانات
        if (!classes.Any())
        {
            throw new InvalidOperationException("لا توجد محاضرات للعرض");
        }

        using var ms = new MemoryStream();
        var document = new Document(PageSize.A4.Rotate(), 20f, 20f, 20f, 20f);
        var writer = PdfWriter.GetInstance(document, ms);
        writer.RunDirection = PdfWriter.RUN_DIRECTION_RTL;

        document.AddCreator("نظام إدارة الطلاب");
        document.AddAuthor("جامعة الفيوم");
        document.AddSubject("جدول المحاضرات العام");
        document.AddTitle("جدول المحاضرات العام");
        document.Open();

        // إعداد الخطوط
        BaseFont baseFont = GetArabicFont();
        var titleFont = new Font(baseFont, 16, Font.BOLD);
        var subtitleFont = new Font(baseFont, 12, Font.BOLD);
        var headerFont = new Font(baseFont, 10, Font.BOLD, BaseColor.WHITE);
        var cellFont = new Font(baseFont, 9, Font.NORMAL);
        var dayFont = new Font(baseFont, 10, Font.BOLD);
        var divisionTitleFont = new Font(baseFont, 14, Font.BOLD, new BaseColor(0, 51, 102)); // أزرق داكن
        var semesterTitleFont = new Font(baseFont, 12, Font.BOLD, new BaseColor(102, 0, 51)); // بنفسجي داكن

        // إضافة الهيدر العام
        AddGeneralScheduleHeader(document, titleFont, subtitleFont, baseFont, filterInfo);

        // تجميع المحاضرات حسب الشعبة أولاً ثم حسب السيمستر
        var classesGroupedByDivisionAndSemester = classes
            .GroupBy(c => c.DivisionName ?? "عام")
            .OrderBy(g => g.Key)
            .ToDictionary(
                divisionGroup => divisionGroup.Key,
                divisionGroup => divisionGroup
                    .GroupBy(c => GetSemesterFromLevel(c.Level ?? "غير محدد"))
                    .Where(semesterGroup => semesterGroup.Key > 0) // فقط السيمسترات الصحيحة
                    .OrderBy(semesterGroup => semesterGroup.Key)
                    .ToList()
            );

        bool isFirstDivision = true;

        foreach (var divisionGroup in classesGroupedByDivisionAndSemester)
        {
            string divisionName = divisionGroup.Key;
            var semesterGroups = divisionGroup.Value;

            if (!semesterGroups.Any()) continue;

            // إضافة صفحة جديدة للشعبة (ما عدا الأولى)
            if (!isFirstDivision)
            {
                document.NewPage();
            }
            isFirstDivision = false;

            // عنوان الشعبة
            AddDivisionTitle(document, divisionName, divisionTitleFont, baseFont);

            // إحصائيات عامة للشعبة
            var allDivisionClasses = semesterGroups.SelectMany(sg => sg).ToList();
            AddDivisionStatistics(document, allDivisionClasses, baseFont);

            bool isFirstSemester = true;
            foreach (var semesterGroup in semesterGroups)
            {
                int semesterNumber = semesterGroup.Key;
                var semesterClasses = semesterGroup.ToList();

                // إضافة مسافة بين السيمسترات (ما عدا الأول)
                if (!isFirstSemester)
                {
                    document.Add(new Paragraph(" ", new Font(baseFont, 10)));
                }
                isFirstSemester = false;

                // عنوان السيمستر
                AddSemesterTitle(document, semesterNumber, semesterTitleFont, baseFont);

                // إحصائيات السيمستر
                AddSemesterStatistics(document, semesterClasses, baseFont);

                // إنشاء جدول المحاضرات للسيمستر
                var semesterScheduleTable = CreateWeeklyScheduleTable(semesterClasses, headerFont, cellFont, dayFont, baseFont);
                document.Add(semesterScheduleTable);

                // إضافة قائمة بأسماء الأساتذة والمواد للسيمستر
                AddSemesterCoursesAndProfessors(document, semesterClasses, baseFont);
            }
        }

        // إضافة الفوتر العام
        AddScheduleFooterInfo(document, classes, cellFont, baseFont);

        document.Close();
        return ms.ToArray();
    }

    private void AddDivisionTitle(Document document, string divisionName, Font divisionTitleFont, BaseFont baseFont)
    {
        // إطار ملون لعنوان الشعبة
        var divisionTitleTable = new PdfPTable(1)
        {
            RunDirection = PdfWriter.RUN_DIRECTION_RTL,
            WidthPercentage = 100,
            SpacingBefore = 15f,
            SpacingAfter = 10f
        };

        var divisionTitleCell = new PdfPCell(new Phrase($"شعبة {divisionName}", divisionTitleFont))
        {
            RunDirection = PdfWriter.RUN_DIRECTION_RTL,
            HorizontalAlignment = Element.ALIGN_CENTER,
            VerticalAlignment = Element.ALIGN_MIDDLE,
            BackgroundColor = new BaseColor(217, 237, 247), // أزرق فاتح
            BorderColor = new BaseColor(0, 51, 102), // أزرق داكن
            BorderWidth = 2f,
            Padding = 12,
            MinimumHeight = 40f
        };

        divisionTitleTable.AddCell(divisionTitleCell);
        document.Add(divisionTitleTable);
    }

    private void AddSemesterTitle(Document document, int semesterNumber, Font semesterTitleFont, BaseFont baseFont)
    {
        var semesterTitleTable = new PdfPTable(1)
        {
            RunDirection = PdfWriter.RUN_DIRECTION_RTL,
            WidthPercentage = 80,
            SpacingBefore = 10f,
            SpacingAfter = 8f,
            HorizontalAlignment = Element.ALIGN_CENTER
        };

        string levelName = GetLevelNameFromSemester(semesterNumber);
        var semesterTitleCell = new PdfPCell(new Phrase($"الفصل الدراسي {semesterNumber} - {levelName}", semesterTitleFont))
        {
            RunDirection = PdfWriter.RUN_DIRECTION_RTL,
            HorizontalAlignment = Element.ALIGN_CENTER,
            VerticalAlignment = Element.ALIGN_MIDDLE,
            BackgroundColor = new BaseColor(240, 240, 240), // رمادي فاتح
            BorderColor = new BaseColor(102, 0, 51), // بنفسجي داكن
            BorderWidth = 1f,
            Padding = 8,
            MinimumHeight = 30f
        };

        semesterTitleTable.AddCell(semesterTitleCell);
        document.Add(semesterTitleTable);
    }

    private void AddDivisionStatistics(Document document, List<ClassDto> divisionClasses, BaseFont baseFont)
    {
        var statsTable = new PdfPTable(5)
        {
            RunDirection = PdfWriter.RUN_DIRECTION_RTL,
            WidthPercentage = 100,
            SpacingAfter = 10f
        };
        statsTable.SetWidths(new float[] { 1f, 1f, 1f, 1f, 1f });

        var totalClasses = divisionClasses.Count;
        var totalCourses = divisionClasses.Select(c => c.CourseName).Distinct().Count();
        var totalProfessors = divisionClasses.Select(c => c.ProfessorName).Distinct().Count();
        var totalSemesters = divisionClasses.Select(c => GetSemesterFromLevel(c.Level ?? "غير محدد")).Distinct().Count();
        var totalLevels = divisionClasses.Select(c => c.Level).Distinct().Count();

        var headerColor = new BaseColor(91, 155, 213);
        var statsFont = new Font(baseFont, 9, Font.BOLD, BaseColor.WHITE);

        // Headers
        statsTable.AddCell(CreateStatsCell("إجمالي المحاضرات", statsFont, headerColor));
        statsTable.AddCell(CreateStatsCell("عدد المواد", statsFont, headerColor));
        statsTable.AddCell(CreateStatsCell("عدد الأساتذة", statsFont, headerColor));
        statsTable.AddCell(CreateStatsCell("عدد السيمسترات", statsFont, headerColor));
        statsTable.AddCell(CreateStatsCell("عدد الفرق", statsFont, headerColor));

        // Values
        var valueFont = new Font(baseFont, 9, Font.NORMAL);
        statsTable.AddCell(CreateStatsCell(totalClasses.ToString(), valueFont, BaseColor.WHITE));
        statsTable.AddCell(CreateStatsCell(totalCourses.ToString(), valueFont, BaseColor.WHITE));
        statsTable.AddCell(CreateStatsCell(totalProfessors.ToString(), valueFont, BaseColor.WHITE));
        statsTable.AddCell(CreateStatsCell(totalSemesters.ToString(), valueFont, BaseColor.WHITE));
        statsTable.AddCell(CreateStatsCell(totalLevels.ToString(), valueFont, BaseColor.WHITE));

        document.Add(statsTable);
    }

    private void AddSemesterStatistics(Document document, List<ClassDto> semesterClasses, BaseFont baseFont)
    {
        var statsTable = new PdfPTable(3)
        {
            RunDirection = PdfWriter.RUN_DIRECTION_RTL,
            WidthPercentage = 60,
            SpacingAfter = 8f,
            HorizontalAlignment = Element.ALIGN_CENTER
        };
        statsTable.SetWidths(new float[] { 1f, 1f, 1f });

        var totalClasses = semesterClasses.Count;
        var totalCourses = semesterClasses.Select(c => c.CourseName).Distinct().Count();
        var totalProfessors = semesterClasses.Select(c => c.ProfessorName).Distinct().Count();

        var headerColor = new BaseColor(155, 187, 89);
        var statsFont = new Font(baseFont, 8, Font.BOLD, BaseColor.WHITE);

        // Headers
        statsTable.AddCell(CreateStatsCell("محاضرات", statsFont, headerColor));
        statsTable.AddCell(CreateStatsCell("مواد", statsFont, headerColor));
        statsTable.AddCell(CreateStatsCell("أساتذة", statsFont, headerColor));

        // Values
        var valueFont = new Font(baseFont, 8, Font.NORMAL);
        statsTable.AddCell(CreateStatsCell(totalClasses.ToString(), valueFont, BaseColor.WHITE));
        statsTable.AddCell(CreateStatsCell(totalCourses.ToString(), valueFont, BaseColor.WHITE));
        statsTable.AddCell(CreateStatsCell(totalProfessors.ToString(), valueFont, BaseColor.WHITE));

        document.Add(statsTable);
    }

    private void AddSemesterCoursesAndProfessors(Document document, List<ClassDto> semesterClasses, BaseFont baseFont)
    {
        document.Add(new Paragraph(" ", new Font(baseFont, 4))); // مسافة صغيرة

        var infoTable = new PdfPTable(2)
        {
            RunDirection = PdfWriter.RUN_DIRECTION_RTL,
            WidthPercentage = 100,
            SpacingBefore = 5f,
            SpacingAfter = 10f
        };
        infoTable.SetWidths(new float[] { 1f, 1f });

        // قائمة المواد
        var coursesCell = new PdfPCell()
        {
            BorderWidth = 0.5f,
            BorderColor = BaseColor.GRAY,
            HorizontalAlignment = Element.ALIGN_RIGHT,
            VerticalAlignment = Element.ALIGN_TOP,
            Padding = 6,
            RunDirection = PdfWriter.RUN_DIRECTION_RTL
        };

        var coursesParagraph = new Paragraph();
        coursesParagraph.Add(new Phrase("المواد:", new Font(baseFont, 9, Font.BOLD)));
        coursesParagraph.Add(Chunk.NEWLINE);

        var courses = semesterClasses.Select(c => c.CourseName).Distinct().OrderBy(c => c).ToList();
        for (int i = 0; i < courses.Count; i++)
        {
            coursesParagraph.Add(new Phrase($"{i + 1}. {courses[i]}", new Font(baseFont, 7, Font.NORMAL)));
            if (i < courses.Count - 1) coursesParagraph.Add(Chunk.NEWLINE);
        }

        coursesParagraph.Alignment = Element.ALIGN_RIGHT;
        coursesCell.AddElement(coursesParagraph);

        // قائمة الأساتذة
        var professorsCell = new PdfPCell()
        {
            BorderWidth = 0.5f,
            BorderColor = BaseColor.GRAY,
            HorizontalAlignment = Element.ALIGN_RIGHT,
            VerticalAlignment = Element.ALIGN_TOP,
            Padding = 6,
            RunDirection = PdfWriter.RUN_DIRECTION_RTL
        };

        var professorsParagraph = new Paragraph();
        professorsParagraph.Add(new Phrase("الأساتذة:", new Font(baseFont, 9, Font.BOLD)));
        professorsParagraph.Add(Chunk.NEWLINE);

        var professors = semesterClasses.Select(c => c.ProfessorName).Distinct().OrderBy(p => p).ToList();
        for (int i = 0; i < professors.Count; i++)
        {
            professorsParagraph.Add(new Phrase($"{i + 1}. {professors[i]}", new Font(baseFont, 7, Font.NORMAL)));
            if (i < professors.Count - 1) professorsParagraph.Add(Chunk.NEWLINE);
        }

        professorsParagraph.Alignment = Element.ALIGN_RIGHT;
        professorsCell.AddElement(professorsParagraph);

        infoTable.AddCell(coursesCell);
        infoTable.AddCell(professorsCell);
        document.Add(infoTable);
    }

    private int GetSemesterFromLevel(string levelName)
    {
        // تحويل اسم الفرقة إلى رقم السيمستر
        return levelName switch
        {
            "الأولى" => 1, // يمكن أن يكون 1 أو 2
            "الثانية" => 3, // يمكن أن يكون 3 أو 4
            "الثالثة" => 5, // يمكن أن يكون 5 أو 6
            "الرابعة" => 7, // يمكن أن يكون 7 أو 8
            _ => 0
        };
    }

    private string GetLevelNameFromSemester(int semester)
    {
        return semester switch
        {
            1 or 2 => "الفرقة الأولى",
            3 or 4 => "الفرقة الثانية",
            5 or 6 => "الفرقة الثالثة",
            7 or 8 => "الفرقة الرابعة",
            _ => "غير محدد"
        };
    }

    
 
    
}