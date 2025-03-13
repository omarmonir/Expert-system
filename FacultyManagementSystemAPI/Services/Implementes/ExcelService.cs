using FacultyManagementSystemAPI.Models.DTOs.Report;
using OfficeOpenXml;

namespace FacultyManagementSystemAPI.Services.Implementes
{
    public class ExcelService
    {
        public async Task<byte[]> GenerateExcelAsync<T>(IEnumerable<T> data, string sheetName)
        {
            using var package = new ExcelPackage();
            var worksheet = package.Workbook.Worksheets.Add(sheetName);

            // Dynamically generate the headers
            var properties = typeof(T).GetProperties();
            for (int i = 0; i < properties.Length; i++)
            {
                worksheet.Cells[1, i + 1].Value = properties[i].Name;
            }

            // Populate the data
            int row = 2;
            foreach (var item in data)
            {
                for (int col = 0; col < properties.Length; col++)
                {
                    worksheet.Cells[row, col + 1].Value = properties[col].GetValue(item);
                }
                row++;
            }

            // Return the generated file as a byte array
            return await Task.FromResult(package.GetAsByteArray());
        }
    }
}
