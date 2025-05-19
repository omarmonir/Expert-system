using AutoMapper;
using FacultyManagementSystemAPI.Data;
using FacultyManagementSystemAPI.Models.DTOs.Auth;
using FacultyManagementSystemAPI.Services.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
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

        public async Task<IEnumerable<UserDto>> GetUsersAsync()
        {
            var users = _userManager.Users.ToList();

            var usersDto = _mapper.Map<List<UserDto>>(users);

            // User لكل  Role لتحديد 
            foreach (var userDto in usersDto)
            {
                var user = users.FirstOrDefault(u => u.Id == userDto.Id);
                userDto.Roles = await _userManager.GetRolesAsync(user);
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

            if (user.UserName == model.UserName && user.PhoneNumber == model.PhoneNumber
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
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
            var random = new Random();
            return new string(Enumerable.Repeat(chars, 10)
                .Select(s => s[random.Next(s.Length)]).ToArray());
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
    }
}