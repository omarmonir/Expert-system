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
        Task<string> AssignRoleAsync(string email, string roleName);
        Task<IEnumerable<UserDto>> GetUsersAsync(int pageNumber);
        Task<UserDto> GetUserByEmailAsync(string email);
        Task<UserDto> GetUserByIdAsync(string Id);
        Task UpdateUserAsync(string Id, UpdateUserDto model);
        Task AddAsync(UserCreateDto userCreateDto);
        Task DeleteAsync(string id);

        //Task<IEnumerable<string>> GetAllRolesAsync();
    }
}
