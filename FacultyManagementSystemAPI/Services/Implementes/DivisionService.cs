using FacultyManagementSystemAPI.Repositories.Implementes;
using FacultyManagementSystemAPI.Repositories.Interfaces;
using FacultyManagementSystemAPI.Services.Interfaces;

namespace FacultyManagementSystemAPI.Services.Implementes
{
    public class DivisionService(IDivisionRepository divisionRepository) : IDivisionService
    {
        public async Task<IEnumerable<string>> GetAllDivisionNameAsync()
        {
            var names = await divisionRepository.GetAllDivisionNameAsync();

            if (names == null || !names.Any())
                throw new Exception("لا يوجد أي شعب");

            return names;
        }
    }
}
