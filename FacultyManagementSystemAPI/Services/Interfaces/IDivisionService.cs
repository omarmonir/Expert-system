namespace FacultyManagementSystemAPI.Services.Interfaces
{
    public interface IDivisionService
    {
        Task<IEnumerable<string>> GetAllDivisionNameAsync();
    }
}
