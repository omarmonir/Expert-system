using AutoMapper;
using FacultyManagementSystemAPI.Data;
using FacultyManagementSystemAPI.Models.DTOs.Auth;
using FacultyManagementSystemAPI.Models.DTOs.Student;
using FacultyManagementSystemAPI.Services.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace FacultyManagementSystemAPI.Services.Implementes
{
    public class AuthService : IAuthService
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IConfiguration _configuration;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly IMapper _mapper;
        private readonly IEmailService _emailService;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly int _refreshTokenValidityInDays;

        public AuthService(
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager,
            RoleManager<IdentityRole> roleManager,
            IConfiguration configuration,
            IMapper mapper,
            IEmailService emailService,
            IHttpContextAccessor httpContextAccessor)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _configuration = configuration;
            _mapper = mapper;
            _emailService = emailService;
            _httpContextAccessor = httpContextAccessor;
            _refreshTokenValidityInDays = configuration.GetValue<int>("Jwt:RefreshTokenValidityInDays");
            _roleManager = roleManager;
        }
        public async Task AddAsync(UserCreateDto userCreateDto)
        {
            if (userCreateDto == null)
                throw new ArgumentNullException(nameof(userCreateDto), "البيانات المدخلة لا يمكن أن تكون فارغة");

            var existingUser = await _userManager.FindByEmailAsync(userCreateDto.Email);

            if (existingUser != null)
                throw new Exception("يوجد مستخدم بهذا البريد الإلكتروني بالفعل");

            var password = GenerateRandomPassword();

            var user = new ApplicationUser
            {
                UserName = userCreateDto.Name,
                PhoneNumber = userCreateDto.Phone,
                Email = userCreateDto.Email,
                UserType = ConstantRoles.Admin,
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

            await _userManager.AddToRoleAsync(user, ConstantRoles.Admin);
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
        <div style='background-color: #003366; padding: 25px; text-align: center;'>
            <h1 style='color: #ffffff; margin: 0; font-size: 26px; font-weight: 600;'>نظام إدارة الكلية</h1>
        </div>

        <div style='padding: 35px 25px;'>
            <h2 style='color: #003366; margin-top: 0; font-size: 22px;'>مرحباً بك في نظام إدارة الكلية</h2>
            <p style='color: #333; font-size: 16px;'>عزيزي <strong>{userCreateDto.Name}</strong>،</p>
            <p style='color: #333; font-size: 16px;'>تم إنشاء حسابك بنجاح بصلاحية <strong>{ConstantRoles.Admin}</strong>. فيما يلي بيانات حسابك:</p>

            <div style='background-color: #eef4fa; border-right: 4px solid #4a7fbf; padding: 22px; margin: 25px 0; border-radius: 6px;'>
                <p style='margin: 10px 0; font-size: 16px;'><strong style='color: #003366;'>البريد الإلكتروني:</strong> {userCreateDto.Email}</p>
                <p style='margin: 10px 0; font-size: 16px;'><strong style='color: #003366;'>نوع الحساب:</strong> {ConstantRoles.Admin}</p>
                <p style='margin: 10px 0; font-size: 16px;'><strong style='color: #003366;'>كلمة المرور:</strong> {password}</p>
            </div>

            <div style='background-color: #fff8e8; border-right: 4px solid #f0b400; padding: 22px; margin: 25px 0; border-radius: 6px;'>
                <h3 style='color: #003366; margin-top: 0; font-size: 18px;'>إرشادات أمنية مهمة:</h3>
                <ul style='padding-right: 20px; color: #444;'>
                    <li style='margin-bottom: 10px;'>لا تشارك بيانات الدخول مع أي شخص</li>
                    <li style='margin-bottom: 10px;'>ستتلقى إشعارات عند تسجيل الدخول من أجهزة جديدة</li>
                    <li style='margin-bottom: 0;'>تأكد من تسجيل الخروج عند استخدام جهاز عام</li>
                </ul>
            </div>

            <p style='font-weight: bold; color: #d93025; text-align: center; font-size: 16px; padding: 10px; background-color: #feeae9; border-radius: 4px;'>⚠ يرجى الحفاظ على بيانات تسجيل الدخول الخاصة بك بشكل آمن</p>

            <div style='background-color: #eef4fa; padding: 20px; border-radius: 6px; margin-top: 25px; border: 1px solid #d0e0f0;'>
                <p style='margin: 0; color: #003366; font-size: 16px;'>هل تحتاج إلى مساعدة؟ تواصل مع الدعم عبر: <a href='mailto:support@college.edu' style='color: #4a7fbf; font-weight: bold;'>support@college.edu</a></p>
            </div>
        </div>

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
                // يمكنك اختيارياً تسجيل الخطأ في نظام التسجيل (Logging) هنا
            }


        }

        public async Task<ResponseLoginDto> LoginAsync(RequestLoginDto request)
        {
            var user = await _userManager.FindByEmailAsync(request.Email);
            if (user == null || !await _userManager.CheckPasswordAsync(user, request.Password))
                throw new Exception("بيانات الاعتماد غير صحيحة");

            if (!user.IsActive || user.LockoutEnd > DateTimeOffset.UtcNow)
                throw new Exception("الحساب معطل حالياً. يرجى التواصل مع الإدارة");

            var roles = await _userManager.GetRolesAsync(user);

            var token = await CreateJwtToken(user);
            string jwtToken = new JwtSecurityTokenHandler().WriteToken(token);

            var refreshToken = GenerateRefreshToken();

            user.RefreshToken = refreshToken;
            user.RefreshTokenExpiryTime = DateTime.UtcNow.AddDays(_refreshTokenValidityInDays);
            user.LastLoginDate = DateTime.UtcNow;
            await _userManager.UpdateAsync(user);

            await SendLoginNotification(user);

            return new ResponseLoginDto
            {
                Token = jwtToken,
                TokenExpiration = token.ValidTo,
                RefreshToken = refreshToken,
                RefreshTokenExpiration = user.RefreshTokenExpiryTime.Value,
                Roles = roles.ToList()
            };
        }

        public async Task<string> AssignRoleAsync(string email, string roleName)
        {
            var user = await _userManager.FindByEmailAsync(email) ??
                throw new Exception("المستخدم غير موجود");

            if (!await _roleManager.RoleExistsAsync(roleName))
                throw new Exception("الدور المطلوب غير موجود في النظام");

            var result = await _userManager.AddToRoleAsync(user, roleName);
            if (!result.Succeeded)
            {
                var errors = string.Join("، ", result.Errors.Select(e => e.Description));
                throw new Exception($"فشل تعيين الدور: {errors}");
            }

            return $"تم تعيين دور '{roleName}' للمستخدم '{email}' بنجاح";
        }

        public async Task<IEnumerable<UserDto>> GetUsersAsync(int pageNumber)
        {
            int pageSize = 20;

            // احصل على المستخدمين بصفحة محددة فقط
            var users = await _userManager.Users
                .OrderBy(u => u.UserName)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            var usersDto = _mapper.Map<List<UserDto>>(users);

            // لكل مستخدم، اجلب الأدوار
            for (int i = 0; i < usersDto.Count; i++)
            {
                usersDto[i].Roles = await _userManager.GetRolesAsync(users[i]);
            }

            return usersDto;
        }

        public async Task<string> LogoutAsync(string email)
        {
            var user = await _userManager.FindByEmailAsync(email);
            if (user == null)
                throw new Exception("المستخدم غير موجود");

            // إبطال جميع الـ Refresh Tokens
            user.RefreshToken = null;
            user.RefreshTokenExpiryTime = null;

            var updateResult = await _userManager.UpdateAsync(user);
            if (!updateResult.Succeeded)
            {
                throw new Exception("فشل في تحديث بيانات المستخدم");
            }

            // تسجيل الخروج من النظام
            await _signInManager.SignOutAsync();

            // إرسال إشعار التسجيل الخروج (بشكل غير متزامن دون انتظار)
           

            return "تم تسجيل الخروج بنجاح";
        }

        public async Task<string> DeactivateAccountAsync(string email)
        {
            var user = await _userManager.FindByEmailAsync(email);
            if (user == null)
                throw new Exception("المستخدم غير موجود");

            user.LockoutEnabled = true;
            user.LockoutEnd = DateTimeOffset.MaxValue;
            user.IsActive = false;
            user.DeactivationDate = DateTime.UtcNow;
            

            var result = await _userManager.UpdateAsync(user);
            if (!result.Succeeded)
                throw new Exception(string.Join(", ", result.Errors.Select(e => e.Description)));

            string subject = "تم تعطيل حسابك";
            string body = $@"
            <h2>إشعار تعطيل الحساب</h2>
            <p>عزيزي المستخدم،</p>
            <p>تم تعطيل حسابك في نظام إدارة الكلية للأسباب التالية:</p>
         
            <p>إذا كنت تعتقد أن هذا خطأ، يرجى التواصل مع الدعم الفني.</p>
            <p>تحياتنا،</p>
            <p><strong>إدارة النظام</strong></p>";

            await _emailService.SendEmailAsync(user.Email, subject, body);

            return "تم تعطيل الحساب بنجاح";
        }

        public async Task<string> ReactivateAccountAsync(string email)
        {
            var user = await _userManager.FindByEmailAsync(email);
            if (user == null)
                throw new Exception("المستخدم غير موجود");

            user.LockoutEnabled = false;
            user.LockoutEnd = null;
            user.IsActive = true;

            var result = await _userManager.UpdateAsync(user);
            if (!result.Succeeded)
                throw new Exception(string.Join(", ", result.Errors.Select(e => e.Description)));

            string subject = "تم إعادة تفعيل حسابك";
            string body = $@"
            <h2>إشعار إعادة التفعيل</h2>
            <p>عزيزي المستخدم،</p>
            <p>تم إعادة تفعيل حسابك في نظام إدارة الكلية.</p>
            <p>يمكنك الآن تسجيل الدخول باستخدام بياناتك المعتادة.</p>
            <p>تحياتنا،</p>
            <p><strong>إدارة النظام</strong></p>";

            await _emailService.SendEmailAsync(user.Email, subject, body);

            return "تم إعادة تفعيل الحساب بنجاح";
        }

        public async Task<string> ResetPasswordAsync(string email)
        {
            var user = await _userManager.FindByEmailAsync(email);
            if (user == null)
                throw new Exception("المستخدم غير موجود");

            string newPassword = GenerateRandomPassword();
            var resetToken = await _userManager.GeneratePasswordResetTokenAsync(user);
            var result = await _userManager.ResetPasswordAsync(user, resetToken, newPassword);

            if (!result.Succeeded)
                throw new Exception(string.Join(", ", result.Errors.Select(e => e.Description)));

            string subject = "إعادة تعيين كلمة المرور";
            string body = $@"
            <h2>تمت إعادة تعيين كلمة المرور الخاصة بك</h2>
            <p>تم تعيين كلمة مرور جديدة لحسابك:</p>
            <p><strong>{newPassword}</strong></p>
            <p>يرجى تسجيل الدخول وتغييرها بعد ذلك للحفاظ على أمان حسابك.</p>";

            await _emailService.SendEmailAsync(user.Email, subject, body);

            return "تمت إعادة تعيين كلمة المرور وإرسالها إلى بريدك الإلكتروني";
        }

        private async Task<JwtSecurityToken> CreateJwtToken(ApplicationUser user)
        {
            var userClaims = await _userManager.GetClaimsAsync(user);
            var roles = await _userManager.GetRolesAsync(user);
            var roleClaims = new List<Claim>();

            foreach (var role in roles)
                roleClaims.Add(new Claim(ClaimTypes.Role, role));

            var claims = new List<Claim>
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.Id),
                new Claim(JwtRegisteredClaimNames.Email, user.Email),
                new Claim("StudentId", user.StudentId?.ToString() ?? ""),
                new Claim("ProfessorId", user.ProfessorId?.ToString() ?? ""),
                new Claim("IsActive", user.IsActive.ToString())
            }.Union(userClaims).Union(roleClaims);

            var symmetricSecurityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]));
            var signingCredentials = new SigningCredentials(symmetricSecurityKey, SecurityAlgorithms.HmacSha256);

            var jwtSecurityToken = new JwtSecurityToken(
                issuer: _configuration["Jwt:Issuer"],
                audience: _configuration["Jwt:Audience"],
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(_configuration.GetValue<int>("Jwt:TokenValidityInMinutes")),
                signingCredentials: signingCredentials);

            return jwtSecurityToken;
        }

        public async Task<UserDto> GetUserByEmailAsync(string email)
        {
            var user = await _userManager.FindByEmailAsync(email)
                 ?? throw new Exception("المستخدم غير موجود");

            var userDto = _mapper.Map<UserDto>(user);
            userDto.Roles = await _userManager.GetRolesAsync(user); // تعيين الأدوار يدويًا

            return userDto;
        }

        public async Task<UserDto> GetUserByIdAsync(string Id)
        {
            var user = await _userManager.FindByIdAsync(Id)
                 ?? throw new Exception("المستخدم غير موجود");

            var userDto = _mapper.Map<UserDto>(user);
            userDto.Roles = await _userManager.GetRolesAsync(user);

            return userDto;
        }

        public async Task UpdateUserAsync(string Id, UpdateUserDto model)
        {
            var user = await _userManager.FindByIdAsync(Id)
                ?? throw new Exception("المستخدم غير موجود");

            if ( user.PhoneNumber == model.PhoneNumber
                && user.Email == model.Email)
                throw new Exception("لم تقم بالتعديل البيانات موجوده بالفعل");

            _mapper.Map(model, user);

            var result = await _userManager.UpdateAsync(user);
            if (!result.Succeeded)
            {
                var errors = string.Join(", ", result.Errors.Select(e => e.Description));
                throw new Exception($"فشل  في التعديل علي المستخدم: {errors}");
            }
        }

        private string GenerateRefreshToken()
        {
            var randomNumber = new byte[64];
            using var rng = RandomNumberGenerator.Create();
            rng.GetBytes(randomNumber);
            return Convert.ToBase64String(randomNumber);
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

        private async Task SendLoginNotification(ApplicationUser user)
        {
            try
            {
                var httpContext = _httpContextAccessor.HttpContext;
                if (httpContext == null) return;

                var deviceInfo = httpContext.Request.Headers["User-Agent"].ToString();
                var ipAddress = httpContext.Connection.RemoteIpAddress?.ToString() ?? "Unknown IP";

                string deviceType = GetDeviceType(deviceInfo);
                string browser = GetBrowserName(deviceInfo);

                string subject = "إشعار تسجيل دخول جديد";
                string body = $@"
                <h2>تم تسجيل دخول إلى حسابك</h2>
                <p>عزيزي المستخدم،</p>
                <p>تم تسجيل دخول إلى حسابك باستخدام:</p>
                <p><strong>نوع الجهاز:</strong> {deviceType}</p>
                <p><strong>المتصفح:</strong> {browser}</p>
                <p><strong>عنوان IP:</strong> {ipAddress}</p>
                <p><strong>الوقت:</strong> {DateTime.Now.ToString("yyyy-MM-dd HH:mm")}</p>
                <p>إذا لم تكن أنت من قام بهذا الإجراء، يرجى تغيير كلمة المرور فورًا.</p>
                <p>تحياتنا،</p>
                <p><strong>إدارة النظام</strong></p>";

                await _emailService.SendEmailAsync(user.Email, subject, body);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error sending login notification: {ex.Message}");
            }
        }

        private string GetDeviceType(string userAgent)
        {
            if (userAgent.Contains("Mobile")) return "هاتف محمول";
            if (userAgent.Contains("Tablet")) return "تابلت";
            if (userAgent.Contains("Windows")) return "كمبيوتر (Windows)";
            if (userAgent.Contains("Mac")) return "كمبيوتر (Mac)";
            if (userAgent.Contains("Linux")) return "كمبيوتر (Linux)";
            return "جهاز غير معروف";
        }

        private string GetBrowserName(string userAgent)
        {
            if (userAgent.Contains("Chrome")) return "Google Chrome";
            if (userAgent.Contains("Firefox")) return "Mozilla Firefox";
            if (userAgent.Contains("Safari")) return "Apple Safari";
            if (userAgent.Contains("Edge")) return "Microsoft Edge";
            if (userAgent.Contains("Opera")) return "Opera";
            return "متصفح غير معروف";
        }
        public async Task DeleteAsync(string id)
        {
            if (id == null)
                throw new ArgumentException("معرف المستخدم مطلوب");

            var user = await _userManager.FindByIdAsync(id)
                ?? throw new KeyNotFoundException("لم يتم العثور على المستخدم");

            var result = await _userManager.DeleteAsync(user);

            if (!result.Succeeded)
            {
                var errors = string.Join(", ", result.Errors.Select(e => e.Description));
                throw new Exception($"فشل حذف المستخدم: {errors}");
            }
        }

    }
}