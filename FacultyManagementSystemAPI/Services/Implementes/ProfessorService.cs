using AutoMapper;
using FacultyManagementSystemAPI.Models.DTOs.Courses;
using FacultyManagementSystemAPI.Models.DTOs.professors;
using FacultyManagementSystemAPI.Models.Entities;
using FacultyManagementSystemAPI.Repositories.Interfaces;
using FacultyManagementSystemAPI.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace FacultyManagementSystemAPI.Services.Implementes
{
    public class ProfessorService(IProfessorRepository professorRepository, IFileService fileService, IMapper mapper) : IProfessorService
    {
        private readonly IProfessorRepository _professorRepository = professorRepository;
        private readonly IFileService _fileService = fileService;
        private readonly IMapper _mapper = mapper;

        public async Task AddAsync(CreateProfessorDto createProfessorDto)
        {
            try
            {
                if (createProfessorDto == null)
                    throw new ArgumentNullException("البيانات المدخلة لا يمكن أن تكون فارغة.");

                if (await _professorRepository.ProfessorExistsAsync(createProfessorDto.FullName))
                    throw new Exception("الدكتور موجود بالفعل.");

                var professor = _mapper.Map<Professor>(createProfessorDto);
                professor.ImagePath = _fileService.SaveFile(createProfessorDto.Image, "Professor");

                await _professorRepository.AddAsync(professor);
            }
            catch (DbUpdateException ex)
            {
                var innerException = ex.InnerException?.Message ?? ex.Message;
                throw new Exception($"حدث خطأ أثناء حفظ التغييرات في قاعدة البيانات: {innerException}");
            }
        }

        public async Task DeleteAsync(int id)
        {
            var professor = await _professorRepository.GetByIdAsync(id)
                 ?? throw new KeyNotFoundException($"لم يتم العثور على دكتور برقم {id}.");
            await _professorRepository.DeleteAsync(id);
        }

        public async Task<IEnumerable<ProfessorDto>> GetAllAsync()
        {
            var professorsDto = await _professorRepository.GetAllProfessorsAsync();
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
            var professorDto = await _professorRepository.GetProfessorByIdAsync(id)
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
            try
            {
                if (id <= 0)
                    throw new ArgumentException("يجب أن يكون معرف الدكنور رقمًا موجبا");

                if (updateProfessorDto == null)
                {
                    throw new ArgumentNullException("بيانات التحديث غير صالحة");
                }
                var professor = await _professorRepository.GetByIdAsync(id)
                            ?? throw new KeyNotFoundException($"لم يتم العثور على الأستاذ برقم {id}.");

                _mapper.Map(updateProfessorDto, professor); // ✅ تحديث آمن
                if (updateProfessorDto.Image != null)
                {
                    // حذف الصورة القديمة إذا كانت موجودة
                    if (!string.IsNullOrEmpty(professor.ImagePath))
                    {
                        await _fileService.DeleteFileAsync(professor.ImagePath);
                    }

                    // حفظ الصورة الجديدة
                    professor.ImagePath = _fileService.SaveFile(updateProfessorDto.Image, "Professors");
                }
                await _professorRepository.UpdateAsync(id, professor);
            }
            catch (DbUpdateException ex)
            {
                var innerException = ex.InnerException?.Message ?? ex.Message;
                throw new Exception($"حدث خطأ أثناء حفظ التغييرات في قاعدة البيانات: {innerException}");
            }
        }
    }
}
