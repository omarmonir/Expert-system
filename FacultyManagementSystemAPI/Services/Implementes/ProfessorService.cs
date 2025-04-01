using AutoMapper;
using FacultyManagementSystemAPI.Data;
using FacultyManagementSystemAPI.Models.DTOs.Courses;
using FacultyManagementSystemAPI.Models.DTOs.professors;
using FacultyManagementSystemAPI.Models.DTOs.Student;
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


        public async Task AddAsync(CreateProfessorDto createProfessorDto)
        {
            if (createProfessorDto == null)
                throw new ArgumentNullException(nameof(createProfessorDto), "البيانات المدخلة لا يمكن أن تكون فارغة");

            // التحقق مما إذا كان الأستاذ موجودًا مسبقًا عبر البريد الإلكتروني
            var existingUser = await _userManager.FindByEmailAsync(createProfessorDto.Email);
            if (existingUser != null)
                throw new Exception("يوجد مستخدم بهذا البريد الإلكتروني بالفعل");

            // إنشاء كلمة مرور عشوائية
            var password = GenerateRandomPassword();

            // 1️⃣ إنشاء المستخدم أولاً
            var user = new ApplicationUser
            {
                UserName = createProfessorDto.FullName,
                PhoneNumber = createProfessorDto.Phone,
                Email = createProfessorDto.Email,
                UserType = "Student",
                StudentId = null, // سيتم تحديثه لاحقًا
                IsActive = true, // الحساب مفعل افتراضيًا
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

            await _userManager.AddToRoleAsync(user, "Professor");

            // 2️⃣ إنشاء الأستاذ وربطه بالمستخدم
            var professor = _mapper.Map<Professor>(createProfessorDto);
            professor.ApplicationUserId = user.Id;
            professor.ImagePath = _fileService.SaveFile(createProfessorDto.Image, "Professor");

            try
            {
                await _professorRepository.AddAsync(professor);

                // 3️⃣ تحديث معرف الأستاذ في المستخدم بعد إضافته
                user.ProfessorId = professor.Id;
                await _userManager.UpdateAsync(user);
            }
            catch (DbUpdateException ex)
            {
                // حذف المستخدم إذا فشلت إضافة الأستاذ
                await _userManager.DeleteAsync(user);
                throw new Exception($"فشل تحديث قاعدة البيانات: {ex.InnerException?.Message}");
            }
            catch (Exception ex)
            {
                // حذف المستخدم إذا فشلت إضافة الأستاذ
                await _userManager.DeleteAsync(user);
                throw new Exception($"حدث خطأ غير متوقع: {ex.Message}");
            }

            // 4️⃣ إرسال البريد الإلكتروني للمستخدم الجديد
            string subject = "تفاصيل حسابك";

            string body = $@"
                        <h2>مرحبًا بك في نظام إدارة الكلية</h2>
                        <p style='font-family: Arial, sans-serif; color: #333;'>عزيزي {createProfessorDto.FullName}،</p>
                        <p style='font-family: Arial, sans-serif; color: #333;'>تم إنشاء حسابك بنجاح. فيما يلي بيانات تسجيل الدخول الخاصة بك:</p>

                        <div style='background-color: #f4f4f9; padding: 15px; border-radius: 8px; margin-bottom: 10px;'>
                            <p style='font-family: Arial, sans-serif; color: #333;'><strong>البريد الإلكتروني:</strong> {createProfessorDto.Email}</p>
                            <p style='font-family: Arial, sans-serif; color: #333;'><strong>كلمة المرور:</strong> {password}</p>
                        </div>

                        <div style='background-color: #fff8e1; padding: 15px; border-radius: 8px; margin-bottom: 10px;'>
                            <h3 style='color: #d32f2f;'>إرشادات أمنية:</h3>
                            <ul>
                              
                                <li>لا تشارك بيانات الدخول مع أي شخص</li>
                                <li>ستتلقى إشعارات عند تسجيل الدخول من أجهزة جديدة</li>
                            </ul>
                        </div>

                        <p style='font-family: Arial, sans-serif; color: #e74c3c; font-weight: bold;'>⚠ يرجى الاحتفاظ بهذه المعلومات بشكل آمن.</p>
                        <p style='font-family: Arial, sans-serif; color: #333;'>تحياتنا،</p>
                        <p style='font-family: Arial, sans-serif; color: #333;'><strong>إدارة النظام</strong></p>";

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
                // يمكنك اختيارياً تسجيل الخطأ في نظام التسجيل (Logging) هنا
            }
        }


        public async Task DeleteAsync(int id)
        {
            var professor = await _professorRepository.GetByIdAsync(id)
                 ?? throw new KeyNotFoundException($"لم يتم العثور على دكتور برقم {id}.");
            await _professorRepository.DeleteAsync(id);
        }

        public async Task<IEnumerable<ProfessorDto>> GetAllAsync()
        {
            var professorsDto = await _professorRepository.GetAllProfessorsAsync();
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

        private string GenerateRandomPassword()
        {
            const string upperCase = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
            const string lowerCase = "abcdefghijklmnopqrstuvwxyz";
            const string digits = "0123456789";
            const string allChars = upperCase + lowerCase + digits;

            var random = new Random();

            // ضمان وجود حرف كبير، حرف صغير، ورقم
            string password =
                upperCase[random.Next(upperCase.Length)].ToString() +
                lowerCase[random.Next(lowerCase.Length)].ToString() +
                digits[random.Next(digits.Length)].ToString();

            // إكمال كلمة المرور بالأحرف العشوائية حتى تصل للطول المطلوب (8)
            password += new string(Enumerable.Repeat(allChars, 5) // 5 لأن لدينا 3 أحرف مضافة مسبقًا
                .Select(s => s[random.Next(s.Length)]).ToArray());

            // خلط الأحرف حتى لا يكون النمط ثابتًا
            return new string(password.OrderBy(_ => random.Next()).ToArray());
        }

    }
}
