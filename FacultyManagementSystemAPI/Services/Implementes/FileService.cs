using FacultyManagementSystemAPI.Services.Interfaces;

namespace FacultyManagementSystemAPI.Services.Implementes
{

    public class FileService : IFileService
	{
		private readonly IWebHostEnvironment _environment;
		private readonly IHttpContextAccessor _httpContextAccessor;
		private readonly string _baseUploadPath;

		public FileService(IWebHostEnvironment environment, IHttpContextAccessor httpContextAccessor)
		{
			_environment = environment;
			_baseUploadPath = Path.Combine(_environment.WebRootPath, "Images");
			_httpContextAccessor = httpContextAccessor;

			if (!Directory.Exists(_baseUploadPath))
			{
				Directory.CreateDirectory(_baseUploadPath);
			}
		}

		public string SaveFile(IFormFile file, string category)
		{
			var allowedExtensions = new[] { ".jpg", ".jpeg", ".png" };
			var extension = Path.GetExtension(file.FileName).ToLower();
			if (!allowedExtensions.Contains(extension))
			{
				throw new InvalidOperationException("نوع الملف غير مدعوم، يُسمح فقط بـ JPG, JPEG, PNG");
			}

			const long maxFileSize = 2 * 1024 * 1024; //2MB
			if (file.Length > maxFileSize)
			{
				throw new InvalidOperationException("حجم الملف يتجاوز الحد الأقصى المسموح به 2MB");
			}

			var categoryPath = Path.Combine(_baseUploadPath, category);
			if (!Directory.Exists(categoryPath))
			{
				Directory.CreateDirectory(categoryPath);
			}

			var fileName = $"{Guid.NewGuid()}{extension}";
			var filePath = Path.Combine(categoryPath, fileName);

			using (var stream = new FileStream(filePath, FileMode.Create))
			{
				file.CopyTo(stream);
			}

			// احصل على عنوان الموقع الحالي
			var request = _httpContextAccessor.HttpContext?.Request;
			if (request == null)
			{
				throw new InvalidOperationException("لا يمكن تحديد عنوان الموقع، تأكد من تمرير HttpContextAccessor بشكل صحيح");
			}
			var baseUrl = $"{request?.Scheme}://{request?.Host}";

			// إنشاء الرابط الكامل للصورة
			return $"{baseUrl}/Images/{category}/{fileName}";
		}

		public async Task DeleteFileAsync(string fileUrl)
		{
			if (string.IsNullOrEmpty(fileUrl))
			{
				throw new ArgumentException("مسار الملف غير صالح");
			}

			// استخراج الجزء النسبي من الرابط
			var relativePath = fileUrl.Replace($"{_httpContextAccessor.HttpContext?.Request.Scheme}://{_httpContextAccessor.HttpContext?.Request.Host}/", "");


			// تحويل المسار إلى مسار فعلي داخل `wwwroot`
			var fullPath = Path.Combine(_environment.WebRootPath, relativePath);

			if (File.Exists(fullPath))
			{
				try
				{
					await Task.Run(() => File.Delete(fullPath));
				}
				catch (Exception ex)
				{
					throw new IOException($"حدث خطأ أثناء حذف الملف: {ex.Message}");
				}
			}
			else
			{
				throw new FileNotFoundException($"لم يتم العثور على الملف: {fullPath}");
			}
		}
	}
}