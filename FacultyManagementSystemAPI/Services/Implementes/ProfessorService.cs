using AutoMapper;
using FacultyManagementSystemAPI.Models.DTOs.Courses;
using FacultyManagementSystemAPI.Models.DTOs.professors;
using FacultyManagementSystemAPI.Models.DTOs.Student;
using FacultyManagementSystemAPI.Models.Entities;
using FacultyManagementSystemAPI.Repositories.Implementes;
using FacultyManagementSystemAPI.Repositories.Interfaces;
using FacultyManagementSystemAPI.Services.Interfaces;

namespace FacultyManagementSystemAPI.Services.Implementes
{
    public class ProfessorService(IProfessorRepository professorRepository, IMapper mapper) : IProfessorService
    {
        private readonly IProfessorRepository _professorRepository = professorRepository;
        private readonly IMapper _mapper = mapper;

        public async Task AddAsync(CreateProfessorDto createProfessorDto)
        {

            if (createProfessorDto == null)
                throw new ArgumentNullException("البيانات المدخلة لا يمكن أن تكون فارغة.");

            if (await _professorRepository.ProfessorExistsAsync(createProfessorDto.FullName))
                throw new Exception("الدكتور موجود بالفعل.");

            var professor = _mapper.Map<Professor>(createProfessorDto);
            await _professorRepository.AddAsync(professor);
        }

        public async Task DeleteAsync(int id)
        {
            var professor = await _professorRepository.GetByIdAsync(id)
                 ?? throw new KeyNotFoundException($"لم يتم العثور على دكتور برقم {id}.");
            await _professorRepository.DeleteAsync(id);
        }

        public async Task<IEnumerable<ProfessorDto>> GetAllAsync()
        {
            var professorsDto = await _professorRepository.GetAllAsync();
            if (professorsDto == null || !professorsDto.Any())
                throw new KeyNotFoundException("لم يتم العثور على أي دكتور.");
            return professorsDto;
        }

        public async Task<IEnumerable<ProfessorDto>> GetByDepartmentIdAsync(int departmentId)
        {
            var professorsDto = await _professorRepository.GetByDepartmentIdAsync(departmentId);
            if (professorsDto == null || !professorsDto.Any())
                throw new KeyNotFoundException("لم يتم العثور على أي دكتور.");
            return professorsDto;
        }

        public async Task<ProfessorDto?> GetByIdAsync(int id)
        {
            var professorDto = await _professorRepository.GetByIdAsync(id)
                ?? throw new KeyNotFoundException($"لم يتم العثور على دكتور برقم {id}.");

            return professorDto;
        }

        public async Task<IEnumerable<CourseDto>> GetCoursesByProfessorIdAsync(int professorId)
        {
            var professorDto = await _professorRepository.GetCoursesByProfessorIdAsync(professorId);
            if (professorDto == null || !professorDto.Any())
                throw new Exception("لا توجد مقررات لهذا الدكتور.");
            return professorDto;
        }

        public async Task UpdateAsync(int id, UpdateProfessorDto updateProfessorDto)
        {
            if (updateProfessorDto == null)
            {
                throw new ArgumentNullException(nameof(updateProfessorDto), "بيانات التحديث غير صالحة");
            }
            var professorDto = await _professorRepository.GetByIdAsync(id)
                        ?? throw new KeyNotFoundException($"لم يتم العثور على الأستاذ برقم {id}.");

            var professor = _mapper.Map<Professor>(professorDto);

            _mapper.Map(updateProfessorDto, professor); // ✅ تحديث آمن
            await _professorRepository.UpdateAsync(id, professor);

        }
    }
}
