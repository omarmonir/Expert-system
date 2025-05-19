namespace FacultyManagementSystemAPI.Models.DTOs.Auth
{
    public class ResponseLoginDto
    {
        public string Token { get; set; }
        public List<String> Roles { get; set; }
        public DateTime TokenExpiration { get; set; }
        public string RefreshToken { get; set; }
        public DateTime RefreshTokenExpiration { get; set; }
    }
}
