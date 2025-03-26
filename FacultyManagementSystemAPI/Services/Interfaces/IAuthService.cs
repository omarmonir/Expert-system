using FacultyManagementSystemAPI.Models.DTOs.Auth;

namespace FacultyManagementSystemAPI.Services.Interfaces
{
    public interface IAuthService
    {
        Task<ResponseLoginDto> LoginAsync(RequestLoginDto request);
        Task<string> ResetPasswordAsync(string email);
        Task<string> LogoutAsync(string email);
        Task<string> DeactivateAccountAsync(string email);
        Task<string> ReactivateAccountAsync(string email);
    }
}
