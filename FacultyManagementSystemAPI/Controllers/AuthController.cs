using FacultyManagementSystemAPI.Models.DTOs.Auth;
using FacultyManagementSystemAPI.Models.DTOs.Student;
using FacultyManagementSystemAPI.Services.Implementes;
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

        [HttpPost("AddUser")]
        //[Authorize(Roles = "Admin, SuperAdmin")]
        public async Task<IActionResult> Add([FromBody] UserCreateDto userCreateDto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                await _authService.AddAsync(userCreateDto);
                return StatusCode(201);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

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
        //[Authorize] // تأكد من وجود هذه السمة
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

        [HttpPost("assign-role")]
        public async Task<IActionResult> AssignRole([FromBody] AssignRoleDto model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                var result = await _authService.AssignRoleAsync(model.Email, model.Role);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("GetUsers")]
        public async Task<IActionResult> GetUsers([FromQuery] int pageNumber = 1)
        {
            try
            {
                var users = await _authService.GetUsersAsync(pageNumber);
                return Ok(users);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("GetUserByEmail/{email}")]
        public async Task<IActionResult> GetUserByEmail(string email)
        {
            try
            {
                var user = await _authService.GetUserByEmailAsync(email);
                return Ok(user);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("GetUserById/{Id}")]
        public async Task<IActionResult> GetUserById(string Id)
        {
            try
            {
                var user = await _authService.GetUserByIdAsync(Id);
                return Ok(user);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPut("UpdateUser/{Id}")]
        public async Task<IActionResult> UpdateUser(string Id, [FromBody] UpdateUserDto model)
        {
            try
            {
                await _authService.UpdateUserAsync(Id, model);
                return NoContent();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        [HttpDelete("DeleteUser/{id}")]
        public async Task<IActionResult> DeleteById(string id)
        {
            try
            {
                await _authService.DeleteAsync(id);
                return NoContent(); // 204 No Content
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message); // 404
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

    }
}
