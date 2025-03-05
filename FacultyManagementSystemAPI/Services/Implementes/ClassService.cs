using AutoMapper;
using FacultyManagementSystemAPI.Models.DTOs.Classes;
using FacultyManagementSystemAPI.Models.DTOs.Courses;
using FacultyManagementSystemAPI.Models.Entities;
using FacultyManagementSystemAPI.Repositories.Implementes;
using FacultyManagementSystemAPI.Repositories.Interfaces;
using FacultyManagementSystemAPI.Services.Interfaces;

namespace FacultyManagementSystemAPI.Services.Implementes
{
    public class ClassService(IClassRepository classRepository, IMapper mapper) : IClassService
	{
		private readonly IMapper _mapper = mapper;
		private readonly IClassRepository _classRepository = classRepository;

        public async Task AddClassAsync(CreateClassDto createClassDto)
        {
            if (createClassDto == null)
                throw new ArgumentNullException("البيانات المدخلة لا يمكن أن تكون فارغة.");

           

            var classes = _mapper.Map<Class>(createClassDto);
            await _classRepository.AddAsync(classes);
        }

        public Task DeleteClassAsync(int id)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<ClassDto>> GetAllClassesAsync()
        {
            throw new NotImplementedException();
        }

        public Task<ClassDto> GetClassByIdAsync(int id)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<ClassDto>> GetClassesByProfessorIdAsync(int professorId)
        {
            throw new NotImplementedException();
        }

        public Task UpdateClassAsync(int id, UpdateClassDto updateClassDto)
        {
            throw new NotImplementedException();
        }
    }
    }
