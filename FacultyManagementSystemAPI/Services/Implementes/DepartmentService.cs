using AutoMapper;
using FacultyManagementSystemAPI.Models.DTOs.Department;
using FacultyManagementSystemAPI.Repositories.Interfaces;
using FacultyManagementSystemAPI.Services.Interfaces;

namespace FacultyManagementSystemAPI.Services.Implementes
{
    public class DepartmentService(IDepartmentRepository departmentRepository, IMapper mapper) : IDepartmentService
    {
        private readonly IDepartmentRepository _departmentRepository = departmentRepository;
        private readonly IMapper _mapper = mapper;


        public async Task<IEnumerable<DepartmentDto>> GetAllAsync()
        {
            var departments = await _departmentRepository.GetAllAsync();

            if (departments == null || !departments.Any())
                throw new Exception("لا يوجد أي أقسام");

            var departmentListDto = _mapper.Map<IEnumerable<DepartmentDto>>(departments);
            return departmentListDto;
        }

        public async Task<DepartmentDto> GetByIdAsync(int id)
        {
            if (id <= 0)
                throw new ArgumentException("يجب أن يكون معرف القسم رقمًا موجبا");
            var department = await _departmentRepository.GetByIdAsync(id) ??
            throw new Exception("القسم غير موجود");

            var departmentDto = _mapper.Map<DepartmentDto>(department);
            return departmentDto;
        }

        public async Task<IEnumerable<string>> GetDepartmentNameAsync()
        {
            var names = await _departmentRepository.GetAllDepartmentNameAsync();

            if (names == null || !names.Any())
                throw new Exception("لا يوجد أي أقسام");

            return names;
        }

        public async Task<int?> GetIdOfDepartmentByNameAsync(string DepartmentName)
        {
            int? departmentId = await _departmentRepository.GetIdOfDepartmentByNameAsync(DepartmentName);
            if (!departmentId.HasValue)
                throw new Exception("القسم غير موجود");

            return departmentId;
        }

    }
}
