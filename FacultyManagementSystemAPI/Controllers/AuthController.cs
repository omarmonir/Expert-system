using FacultyManagementSystemAPI.Models.DTOs.Auth;
using FacultyManagementSystemAPI.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace FacultyManagementSystemAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AppUserController(IAuthService authService) : ControllerBase
    {
        private readonly IAuthService _authService = authService;


        [HttpPost("Login")]
        public async Task<IActionResult> Login([FromBody] RequestLoginDto request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                var result = await _authService.LoginAsync(request);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost("reset-password")]
        public async Task<IActionResult> ResetPassword([FromBody] ForgotPasswordDto dto)
        {
            try
            {
                var message = await _authService.ResetPasswordAsync(dto.Email);
                return Ok(new { message });
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }
        [HttpPost("Logout")]
        [Authorize] // تأكد من وجود هذه السمة
        public async Task<IActionResult> Logout()
        {
            try
            {
                // طريقة أكثر موثوقية للحصول على البريد الإلكتروني
                var email = User.FindFirstValue(ClaimTypes.Email);

                if (string.IsNullOrEmpty(email))
                    return BadRequest(new { error = "لم يتم العثور على بيانات المستخدم" });

                var message = await _authService.LogoutAsync(email);
                return Ok(new { message });
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }

        [HttpPost("deactivate")]
        public async Task<IActionResult> DeactivateAccount([FromBody] DeactivateAccountDto dto)
        {
            try
            {
                var message = await _authService.DeactivateAccountAsync(dto.Email);
                return Ok(new { message });
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }

        [HttpPost("reactivate")]
        public async Task<IActionResult> ReactivateAccount([FromBody] ReactivateAccountDto dto)
        {
            try
            {
                var message = await _authService.ReactivateAccountAsync(dto.Email);
                return Ok(new { message });
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }
    }
}
