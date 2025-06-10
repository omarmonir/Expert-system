using AutoMapper;
using FacultyManagementSystemAPI.Data;
using FacultyManagementSystemAPI.Models.DTOs.Courses;
using FacultyManagementSystemAPI.Models.DTOs.professors;
using FacultyManagementSystemAPI.Models.Entities;
using FacultyManagementSystemAPI.Repositories.Interfaces;
using FacultyManagementSystemAPI.Services.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace FacultyManagementSystemAPI.Services.Implementes
{
    public class ProfessorService(IProfessorRepository professorRepository, IFileService fileService, IMapper mapper
       , UserManager<ApplicationUser> userManager, IEmailService emailService) : IProfessorService
    {
        private readonly IProfessorRepository _professorRepository = professorRepository;
        private readonly IFileService _fileService = fileService;
        private readonly IEmailService _emailService = emailService;
        private readonly IMapper _mapper = mapper;
        private readonly UserManager<ApplicationUser> _userManager = userManager;


        public async Task AddAsync(CreateProfessorDto createProfessorDto)
        {
            if (createProfessorDto == null)
                throw new ArgumentNullException(nameof(createProfessorDto), "البيانات المدخلة لا يمكن أن تكون فارغة");

            // Check if professor exists by email
            var existingUser = await _userManager.FindByEmailAsync(createProfessorDto.Email);
            if (existingUser != null)
                throw new Exception("يوجد مستخدم بهذا البريد الإلكتروني بالفعل");

            // Generate random password
            var password = GenerateRandomPassword();

            // 1️⃣ Create user first
            var user = new ApplicationUser
            {
                UserName = createProfessorDto.Email,
                PhoneNumber = createProfessorDto.Phone,
                Email = createProfessorDto.Email,
                UserType = ConstantRoles.Professor,
                ProfessorId = null, // Will be updated later
                IsActive = true,
                RefreshToken = null,
                RefreshTokenExpiryTime = null,
                LastLoginDate = null,
                LastLoginIp = null,
                LastLoginDevice = null,
                DeactivationDate = null,
            };

            var result = await _userManager.CreateAsync(user, password);
            if (!result.Succeeded)
            {
                var errors = string.Join(", ", result.Errors.Select(e => e.Description));
                throw new Exception($"فشل إنشاء المستخدم: {errors}");
            }

            await _userManager.AddToRoleAsync(user, ConstantRoles.Professor);

            // 2️⃣ Create professor and link to user
            var professor = _mapper.Map<Professor>(createProfessorDto);
            professor.ApplicationUserId = user.Id;
            professor.Join_Date = createProfessorDto.JoinDate;
            professor.IsHeadOfDepartment = false; // Default value

            var department = await _professorRepository.GetDepartmentByNameAsync(createProfessorDto.DepartmentName)
               ?? throw new Exception("القسم المحدد غير موجود");
            var departmentId = department.Id;

            professor.DepartmentId = departmentId;

            // Handle image processing
            if (createProfessorDto.Image != null)
            {
                professor.ImagePath = _fileService.SaveFile(createProfessorDto.Image, ConstantRoles.Professor);
            }
            else
            {
                professor.ImagePath = "Professors/default.png"; // Default image path
            }

            try
            {
                await _professorRepository.AddAsync(professor);

                // 3️⃣ Update professor ID in user after adding
                user.ProfessorId = professor.Id;
                await _userManager.UpdateAsync(user);
            }
            catch (DbUpdateException ex)
            {
                // Delete user if professor creation fails
                await _userManager.DeleteAsync(user);
                throw new Exception($"فشل تحديث قاعدة البيانات: {ex.InnerException?.Message}");
            }
            catch (Exception ex)
            {
                // Delete user if professor creation fails
                await _userManager.DeleteAsync(user);
                throw new Exception($"حدث خطأ غير متوقع: {ex.Message}");
            }

            // 4️⃣ Send email to new professor
            string subject = "مرحباً بك في نظام إدارة الكلية - تفاصيل حسابك";
            string body = $@"
        <!DOCTYPE html>
        <html dir='rtl' lang='ar'>
        <head>
            <meta charset='UTF-8'>
            <meta name='viewport' content='width=device-width, initial-scale=1.0'>
            <title>تفاصيل حسابك</title>
        </head>
        <body style='margin: 0; padding: 0; font-family: Arial, Tahoma, Geneva, Verdana, sans-serif; background-color: #f0f4f8; direction: rtl; text-align: right;'>
            <div style='max-width: 600px; margin: 0 auto; background-color: #ffffff; box-shadow: 0 0 15px rgba(0,0,0,0.1); border-radius: 8px; overflow: hidden;'>
                <!-- Header -->
                <div style='background-color: #003366; padding: 25px; text-align: center;'>
                    <h1 style='color: #ffffff; margin: 0; font-size: 26px; font-weight: 600;'>نظام إدارة الكلية</h1>
                </div>

                <!-- Content -->
                <div style='padding: 35px 25px;'>
                    <h2 style='color: #003366; margin-top: 0; font-size: 22px; text-align: right;'>مرحباً بك في نظام إدارة الكلية</h2>
                    <p style='color: #333; line-height: 1.7; font-size: 16px; text-align: right;'>عزيزي الدكتور <strong>{createProfessorDto.FullName}</strong>،</p>
                    <p style='color: #333; line-height: 1.7; font-size: 16px; text-align: right;'>تم إنشاء حسابك بنجاح. فيما يلي بيانات تسجيل الدخول الخاصة بك:</p>
    
                    <!-- Login Info Box -->
                    <div style='background-color: #eef4fa; border-right: 4px solid #4a7fbf; padding: 22px; margin: 25px 0; border-radius: 6px; text-align: right;'>
                        <p style='margin: 10px 0; font-size: 16px;'><strong style='color: #003366;'>البريد الإلكتروني:</strong> {createProfessorDto.Email}</p>
                        <p style='margin: 10px 0; font-size: 16px;'><strong style='color: #003366;'>كلمة المرور:</strong> {password}</p>
                    </div>
    
                    <!-- Professor Info -->
                    <div style='background-color: #f5f7fa; padding: 22px; margin: 25px 0; border-radius: 6px; border: 1px solid #e0e5eb; text-align: right;'>
                        <h3 style='color: #003366; margin-top: 0; font-size: 18px;'>بيانات الدكتور:</h3>
                        <p style='margin: 8px 0; font-size: 16px;'><strong style='color: #4a7fbf;'>الاسم:</strong> {createProfessorDto.FullName}</p>
                        <p style='margin: 8px 0; font-size: 16px;'><strong style='color: #4a7fbf;'>رقم الهاتف:</strong> {createProfessorDto.Phone}</p>
                        <p style='margin: 8px 0; font-size: 16px;'><strong style='color: #4a7fbf;'>المسمى الوظيفي:</strong> {createProfessorDto.Position}</p>
                        <p style='margin: 8px 0; font-size: 16px;'><strong style='color: #4a7fbf;'>تاريخ الانضمام:</strong> {createProfessorDto.JoinDate.ToString("yyyy-MM-dd")}</p>
                    </div>

                    <!-- Security Alert -->
                    <div style='background-color: #fff8e8; border-right: 4px solid #f0b400; padding: 22px; margin: 25px 0; border-radius: 6px; text-align: right;'>
                        <h3 style='color: #003366; margin-top: 0; font-size: 18px;'>إرشادات أمنية مهمة:</h3>
                        <ul style='padding-right: 20px; margin-bottom: 0; color: #444; text-align: right;'>
                            <li style='margin-bottom: 10px; line-height: 1.6;'>لا تشارك بيانات الدخول مع أي شخص</li>
                            <li style='margin-bottom: 10px; line-height: 1.6;'>ستتلقى إشعارات عند تسجيل الدخول من أجهزة جديدة</li>
                            <li style='margin-bottom: 0; line-height: 1.6;'>تأكد من تسجيل الخروج عند استخدام جهاز عام</li>
                        </ul>
                    </div>
    
                    <!-- Warning -->
                    <p style='font-weight: bold; color: #d93025; margin-top: 30px; text-align: center; font-size: 16px; padding: 10px; background-color: #feeae9; border-radius: 4px;'>⚠ يرجى الاحتفاظ بهذه المعلومات بشكل آمن</p>
    
                    <!-- Need Help -->
                    <div style='background-color: #eef4fa; padding: 20px; border-radius: 6px; margin-top: 25px; border: 1px solid #d0e0f0; text-align: right;'>
                        <p style='margin: 0; color: #003366; font-size: 16px;'>هل تحتاج إلى مساعدة؟ يمكنك التواصل مع الدعم الفني عبر البريد الإلكتروني: <a href='mailto:support@college.edu' style='color: #4a7fbf; text-decoration: none; font-weight: bold;'>support@college.edu</a></p>
                    </div>
                </div>

                <!-- Footer -->
                <div style='background-color: #003366; padding: 25px; text-align: center; border-top: 1px solid #002755;'>
                    <p style='margin: 0; color: #ffffff; font-size: 16px;'>مع تحيات،<br><strong>إدارة نظام الكلية</strong></p>
                    <p style='margin-top: 15px; color: #a0b8d9; font-size: 14px;'>© 2025 نظام إدارة الكلية. جميع الحقوق محفوظة.</p>
                </div>
            </div>
        </body>
        </html>";

            try
            {
                bool emailSent = await _emailService.SendEmailAsync(user.Email, subject, body);
                if (!emailSent)
                {
                    throw new Exception("تم إنشاء الحساب بنجاح، ولكن فشل إرسال البريد الإلكتروني.");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("❌ خطأ في إرسال البريد: " + ex.Message);
                // Optionally log the error
            }
        }

        public async Task AddMultipleAsync(CreateProfessorDto createProfessorDto)
        {
            if (createProfessorDto == null)
                throw new ArgumentNullException(nameof(createProfessorDto), "البيانات المدخلة لا يمكن أن تكون فارغة");

            // Check if professor exists by email
            var existingUser = await _userManager.FindByEmailAsync(createProfessorDto.Email);
            if (existingUser != null)
                throw new Exception("يوجد مستخدم بهذا البريد الإلكتروني بالفعل");

            // Generate random password
            var password = GenerateRandomPassword();

            // 1️⃣ Create user first
            var user = new ApplicationUser
            {
                UserName = createProfessorDto.Email,
                PhoneNumber = createProfessorDto.Phone,
                Email = createProfessorDto.Email,
                UserType = ConstantRoles.Professor,
                ProfessorId = null, // Will be updated later
                IsActive = true,
                RefreshToken = null,
                RefreshTokenExpiryTime = null,
                LastLoginDate = null,
                LastLoginIp = null,
                LastLoginDevice = null,
                DeactivationDate = null,
            };

            var result = await _userManager.CreateAsync(user, password);
            if (!result.Succeeded)
            {
                var errors = string.Join(", ", result.Errors.Select(e => e.Description));
                throw new Exception($"فشل إنشاء المستخدم: {errors}");
            }

            await _userManager.AddToRoleAsync(user, ConstantRoles.Professor);

            // 2️⃣ Create professor and link to user
            var professor = _mapper.Map<Professor>(createProfessorDto);
            professor.ApplicationUserId = user.Id;
            professor.Join_Date = createProfessorDto.JoinDate;
            professor.IsHeadOfDepartment = false; // Default value

            var department = await _professorRepository.GetDepartmentByNameAsync(createProfessorDto.DepartmentName)
               ?? throw new Exception("القسم المحدد غير موجو");
            var departmentId = department.Id;

            professor.DepartmentId = departmentId;

            // Handle image processing
            if (createProfessorDto.Image != null)
            {
                professor.ImagePath = _fileService.SaveFile(createProfessorDto.Image, ConstantRoles.Professor);
            }
            else
            {
                professor.ImagePath = "Professors/default.png"; // Default image path
            }

            try
            {
                await _professorRepository.AddAsync(professor);

                // 3️⃣ Update professor ID in user after adding
                user.ProfessorId = professor.Id;
                await _userManager.UpdateAsync(user);
            }
            catch (DbUpdateException ex)
            {
                // Delete user if professor creation fails
                await _userManager.DeleteAsync(user);
                throw new Exception($"فشل تحديث قاعدة البيانات: {ex.InnerException?.Message}");
            }
            catch (Exception ex)
            {
                // Delete user if professor creation fails
                await _userManager.DeleteAsync(user);
                throw new Exception($"حدث خطأ غير متوقع: {ex.Message}");
            }

        }

        //public async Task AddAsync(CreateProfessorDto createProfessorDto)
        //{
        //    try
        //    {
        //        if (createProfessorDto == null)
        //            throw new ArgumentNullException("البيانات المدخلة لا يمكن أن تكون فارغة.");

        //        if (await _professorRepository.ProfessorExistsAsync(createProfessorDto.FullName))
        //            throw new Exception("الدكتور موجود بالفعل.");

        //        var professor = _mapper.Map<Professor>(createProfessorDto);
        //        professor.ImagePath = _fileService.SaveFile(createProfessorDto.Image, "Professor");

        //        await _professorRepository.AddAsync(professor);
        //    }
        //    catch (DbUpdateException ex)
        //    {
        //        var innerException = ex.InnerException?.Message ?? ex.Message;
        //        throw new Exception($"حدث خطأ أثناء حفظ التغييرات في قاعدة البيانات: {innerException}");
        //    }
        //}

        public async Task DeleteAsync(int id)
        {
            var professor = await _professorRepository.GetByIdAsync(id)
                 ?? throw new KeyNotFoundException($"لم يتم العثور على دكتور برقم {id}.");
            await _professorRepository.DeleteAsync(id);
        }

        public async Task<IEnumerable<ProfessorDto>> GetAllAsync(int pageNumber)
        {
            var professorsDto = await _professorRepository.GetAllProfessorsAsync(pageNumber);
            if (professorsDto == null || !professorsDto.Any())
                throw new KeyNotFoundException("لم يتم العثور على أي دكتور.");
            return professorsDto;
        }

        public async Task<IEnumerable<string>> GetAllProfessorsNameAsync()
        {

            var names = await _professorRepository.GetAllProfessorsNameAsync();

            if (names == null || !names.Any())
                throw new Exception("لا يوجد أي دكاترة");

            return names;
        }

        public async Task<IEnumerable<ProfessorDto>> GetByDepartmentIdAsync(int departmentId)
        {
            var professorsDto = await _professorRepository.GetByDepartmentIdAsync(departmentId);
            if (professorsDto == null || !professorsDto.Any())
                throw new KeyNotFoundException("لم يتم العثور على أي دكتور.");
            return professorsDto;
        }

        public async Task<ProfessorDto?> GetByIdAsync(int id)
        {
            var professorDto = await _professorRepository.GetProfessorByIdAsync(id)
                ?? throw new KeyNotFoundException($"لم يتم العثور على دكتور برقم {id}.");

            return professorDto;
        }

        public async Task<IEnumerable<CourseDto>> GetCoursesByProfessorIdAsync(int professorId)
        {
            var professorDto = await _professorRepository.GetCoursesByProfessorIdAsync(professorId);
            if (professorDto == null || !professorDto.Any())
                throw new Exception("لا توجد مقررات لهذا الدكتور.");
            return professorDto;
        }

        public async Task UpdateAsync(int id, UpdateProfessorDto updateProfessorDto)
        {
            try
            {
                if (id <= 0)
                    throw new ArgumentException("يجب أن يكون معرف الدكنور رقمًا موجبا");

                if (updateProfessorDto == null)
                {
                    throw new ArgumentNullException("بيانات التحديث غير صالحة");
                }
                var professor = await _professorRepository.GetByIdAsync(id)
                            ?? throw new KeyNotFoundException($"لم يتم العثور على الأستاذ برقم {id}.");

                _mapper.Map(updateProfessorDto, professor); // ✅ تحديث آمن
                if (updateProfessorDto.Image != null)
                {
                    // حذف الصورة القديمة إذا كانت موجودة
                    if (!string.IsNullOrEmpty(professor.ImagePath))
                    {
                        await _fileService.DeleteFileAsync(professor.ImagePath);
                    }

                    // حفظ الصورة الجديدة
                    professor.ImagePath = _fileService.SaveFile(updateProfessorDto.Image, "Professors");
                }
                await _professorRepository.UpdateAsync(id, professor);
            }
            catch (DbUpdateException ex)
            {
                var innerException = ex.InnerException?.Message ?? ex.Message;
                throw new Exception($"حدث خطأ أثناء حفظ التغييرات في قاعدة البيانات: {innerException}");
            }
        }

        //private string GenerateRandomPassword()
        //{
        //    const string upperCase = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
        //    const string lowerCase = "abcdefghijklmnopqrstuvwxyz";
        //    const string digits = "0123456789";
        //    const string allChars = upperCase + lowerCase + digits;

        //    var random = new Random();

        //    // ضمان وجود حرف كبير، حرف صغير، ورقم
        //    string password =
        //        upperCase[random.Next(upperCase.Length)].ToString() +
        //        lowerCase[random.Next(lowerCase.Length)].ToString() +
        //        digits[random.Next(digits.Length)].ToString();

        //    // إكمال كلمة المرور بالأحرف العشوائية حتى تصل للطول المطلوب (8)
        //    password += new string(Enumerable.Repeat(allChars, 5) // 5 لأن لدينا 3 أحرف مضافة مسبقًا
        //        .Select(s => s[random.Next(s.Length)]).ToArray());

        //    // خلط الأحرف حتى لا يكون النمط ثابتًا
        //    return new string(password.OrderBy(_ => random.Next()).ToArray());
        //}
        private string GenerateRandomPassword()
        {
            const string allChars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz";
            const string digitChars = "0123456789";

            var random = new Random();
            var passwordChars = new List<char>();

            passwordChars.Add(digitChars[random.Next(digitChars.Length)]);

            for (int i = 0; i < 7; i++)
            {
                passwordChars.Add(allChars[random.Next(allChars.Length)]);
            }

            return new string(passwordChars.OrderBy(x => random.Next()).ToArray());
        }
    }
}
