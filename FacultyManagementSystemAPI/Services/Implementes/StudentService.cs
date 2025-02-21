using AutoMapper;
using FacultyManagementSystemAPI.Models.DTOs.Student;
using FacultyManagementSystemAPI.Models.Entities;
using FacultyManagementSystemAPI.Repositories.Interfaces;
using FacultyManagementSystemAPI.Services.Interfaces;

namespace FacultyManagementSystemAPI.Services.Implementes
{
	public class StudentService(IStudentRepository studentRepository, IFileService fileService, IMapper mapper) : IStudentService
	{
		private readonly IStudentRepository _studentRepository = studentRepository;
		private readonly IFileService _fileService = fileService;
		private readonly IMapper _mapper = mapper;

		public async Task<IEnumerable<StudentDto>> GetAllWithDepartmentNameAsync()
		{
			var studentsDto = await _studentRepository.GetAllWithDepartmentNameAsync();
			if (studentsDto == null || !studentsDto.Any())
				throw new Exception("No Students Existing!");

			return studentsDto;
		}

		public async Task AddAsync(CreateStudentDto createStudentDto)
		{
			if (createStudentDto == null) throw new ArgumentNullException("Input data cannot be null");

			await ValidateStudentData(createStudentDto);

			var student = _mapper.Map<Student>(createStudentDto);

			student.ImagePath = _fileService.SaveFile(createStudentDto.Image, "Students")!;

			await _studentRepository.AddAsync(student);
		}

		public Task DeleteAsync(int id)
		{
			throw new NotImplementedException();
		}

		public Task<IEnumerable<StudentDto>> GetAllAsync()
		{
			throw new NotImplementedException();
		}

		public async Task<StudentDto> GetByIdWithDepartmentNameAsync(int id)
		{
			var student = await _studentRepository.GetByIdWithDepartmentNameAsync(id) ??
				throw new Exception("الطالب غير موجود");

			var studentSto = _mapper.Map<StudentDto>(student);

			return studentSto;
		}

		public Task UpdateAsync(int id, UpdateStudentDto updateStudentDto)
		{
			throw new NotImplementedException();
		}

		private async Task ValidateStudentData(CreateStudentDto studentDto)
		{
			if (!await _studentRepository.DepartmentExistsAsync(studentDto.DepartmentId))
			{
				throw new KeyNotFoundException("Department not found!");
			}

			if (await _studentRepository.EmailExistsAsync(studentDto.Email))
			{
				throw new InvalidOperationException("Email already exists!");
			}

			if (await _studentRepository.PhoneExistsAsync(studentDto.Phone))
			{
				throw new InvalidOperationException("Phone Number already exists!");
			}
		}

	}
}