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
				throw new Exception("لا يوجد طلاب");

			return studentsDto;
		}

		public async Task AddAsync(CreateStudentDto createStudentDto)
		{
			if (createStudentDto == null)
				throw new ArgumentNullException(nameof(createStudentDto), "البيانات المدخلة لا يمكن أن تكون فارغة");


			await ValidateStudentData(createStudentDto);

			var student = _mapper.Map<Student>(createStudentDto);

			student.ImagePath = _fileService.SaveFile(createStudentDto.Image, "Students");

			await _studentRepository.AddAsync(student);
		}

		public async Task DeleteAsync(int id)
		{
			// البحث عن الطالب في قاعدة البيانات
			var student = await _studentRepository.GetByIdAsync(id) ??
				throw new KeyNotFoundException("لم يتم العثور على الطالب"); ;

			// حذف الصورة إذا كانت موجودة
			if (!string.IsNullOrEmpty(student.ImagePath))
			{
				await _fileService.DeleteFileAsync(student.ImagePath);
			}

			// حذف الطالب من قاعدة البيانات
			await _studentRepository.DeleteAsync(id);
		}

		public async Task<StudentDto> GetByIdWithDepartmentNameAsync(int id)
		{
			var student = await _studentRepository.GetByIdWithDepartmentNameAsync(id) ??
				throw new Exception("الطالب غير موجود");

			var studentSto = _mapper.Map<StudentDto>(student);

			return studentSto;
		}

		public async Task UpdateAsync(int id, UpdateStudentDto updateStudentDto)
		{
			if (updateStudentDto == null)
			{
				throw new ArgumentNullException(nameof(updateStudentDto), "بيانات التحديث غير صالحة");
			}

			var existingStudent = await _studentRepository.GetByIdAsync(id) ??
				throw new KeyNotFoundException($"لم يتم العثور على الطالب بالرقم التعريفي {id}");

			//var createdStudent = _mapper.Map<CreateStudentDto>(updateStudentDto);
			//await ValidateStudentData(createdStudent);

			_mapper.Map(updateStudentDto, existingStudent);

			// تحديث الصورة إذا تم تحميل صورة جديدة
			if (updateStudentDto.Image != null)
			{
				// حذف الصورة القديمة إذا كانت موجودة
				if (!string.IsNullOrEmpty(existingStudent.ImagePath))
				{
					await _fileService.DeleteFileAsync(existingStudent.ImagePath);
				}

				// حفظ الصورة الجديدة
				existingStudent.ImagePath = _fileService.SaveFile(updateStudentDto.Image, "Students");
			}
			await _studentRepository.UpdateAsync(id, existingStudent);
		}

		private async Task ValidateStudentData(CreateStudentDto studentDto)
		{
			if (!await _studentRepository.DepartmentExistsAsync(studentDto.DepartmentId))
			{
				throw new KeyNotFoundException("لم يتم العثور على القسم");
			}

			if (await _studentRepository.EmailExistsAsync(studentDto.Email))
			{
				throw new InvalidOperationException("البريد الإلكتروني مستخدم من قبل");
			}

			if (await _studentRepository.PhoneExistsAsync(studentDto.Phone))
			{
				throw new InvalidOperationException("رقم الهاتف مستخدم من قبل");
			}
		}

		public async Task<IEnumerable<StudentDto>> GetByNameWithDepartmentNameAsync(string name)
		{
			var studentsDto = await _studentRepository.GetByNameWithDepartmentNameAsync(name);

			if (studentsDto == null || !studentsDto.Any())
			{
				throw new Exception("لا يوجد طلاب");
			}

			return studentsDto;
		}
		public async Task<StudentWithGradesDto> GetByIdWithHisGradeAsync(int id)
		{
			var gradesDto = await _studentRepository.GetByIdWithHisGradeAsync(id) ??
				 throw new Exception("الطالب غير موجود");

			return gradesDto;
		}
	}
}