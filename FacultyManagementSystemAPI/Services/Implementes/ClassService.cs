using AutoMapper;
using FacultyManagementSystemAPI.Models.DTOs.Classes;
using FacultyManagementSystemAPI.Models.DTOs.Courses;
using FacultyManagementSystemAPI.Models.Entities;
using FacultyManagementSystemAPI.Repositories.Implementes;
using FacultyManagementSystemAPI.Repositories.Interfaces;
using FacultyManagementSystemAPI.Services.Interfaces;
using Gherkin.CucumberMessages.Types;
using Microsoft.EntityFrameworkCore;

namespace FacultyManagementSystemAPI.Services.Implementes
{
    //public class ClassService(IClassRepository classRepository, IMapper mapper, ICourseRepository courseRepository) : IClassService
    //{
    //    private readonly IClassRepository _classRepository = classRepository;
    //    private readonly ICourseRepository _courseRepository = courseRepository;

    //    private readonly IMapper _mapper = mapper;

    //    public async Task AssignCourseToProfessorAsync(int courseId, string professorName)
    //    {
    //        var professor = await _classRepository.GetProfessorByNameAsync(professorName) ??
    //            throw new KeyNotFoundException($"الأستاذ '{professorName}' غير موجود.");

    //        var course = await _courseRepository.GetByIdAsync(courseId) ??
    //            throw new KeyNotFoundException($"الكورس بالرقم التعريفي {courseId} غير موجود.");

    //        var existingClass = await _classRepository.GetClassByProfessorNameAsync(professorName) ??
    //            throw new KeyNotFoundException($"لا يوجد صف دراسي مرتبط بالأستاذ '{professorName}'.");

    //        bool isAlreadyAssigned = await _classRepository.IsCourseAlreadyAssignedAsync(courseId, professor.Id);
    //        if (isAlreadyAssigned)
    //        {
    //            throw new InvalidOperationException($"الكورس '{course.Name}' تم تعيينه بالفعل للأستاذ '{professorName}'.");
    //        }

    //        existingClass.CourseId = courseId;

    //        await _classRepository.UpdateClassAsync(existingClass);
    //    }

    //    public async Task<IEnumerable<ClassDto>> GetAllClassesWithProfNameAndCourseNameAsync()
    //    {
    //        var classesDto = await _classRepository.GetAllClassesWithProfNameAndCourseNameAsync();

    //        if (classesDto == null || !classesDto.Any())
    //            throw new KeyNotFoundException("لم يتم العثور على أي محاضرات.");

    //        return _mapper.Map<IEnumerable<ClassDto>>(classesDto);
    //    }

    //    public async Task<ClassDto> GetClassByIdAsync(int id)
    //    {
    //        var classDto = await _classRepository.GetClassByIdAsync(id)
    //            ?? throw new KeyNotFoundException($"لم يتم العثور على المحاضرة برقم {id}.");


    //        return _mapper.Map<ClassDto>(classDto);
    //    }

    //    public async Task CreateClassAsync(CreateClassDto createClassDto)
    //    {
           
    //            if (createClassDto == null)
    //                throw new ArgumentNullException("البيانات المدخلة لا يمكن أن تكون فارغة.");

    //            // التحقق من تعارض الوقت والمكان
    //            bool isConflict = await _classRepository.IsTimeAndLocationConflictAsync(
    //                createClassDto.StartTime,
    //                createClassDto.EndTime,
    //                createClassDto.Day,
    //                createClassDto.Location
    //            );

    //            if (isConflict)
    //                throw new Exception("هناك محاضرة أخرى في نفس المكان والوقت.");


    //            bool isAlreadyAssigned = await _classRepository.IsCourseAlreadyAssignedAsync(createClassDto.CourseId, createClassDto.ProfessorId);
    //            if (isAlreadyAssigned)
    //            {
    //                throw new InvalidOperationException($"الكورس موجود بالفعل.");
    //            }

    //            // إنشاء الفصل
    //            var classEntity = _mapper.Map<Class>(createClassDto);
    //            await _classRepository.AddAsync(classEntity);
            
    //    }

    //    public async Task UpdateClassAsync(int id, UpdateClassDto updateClassDto)
    //    {
           
    //            if (updateClassDto == null)
    //                throw new ArgumentNullException("البيانات المدخلة لا يمكن أن تكون فارغة.");

    //            // التحقق من تعارض الوقت والمكان
    //            bool isConflict = await _classRepository.IsTimeAndLocationConflictAsync(
    //                updateClassDto.StartTime,
    //                updateClassDto.EndTime,
    //                updateClassDto.Day,
    //                updateClassDto.Location
    //            );

    //            if (isConflict)
    //                throw new Exception("هناك محاضرة أخرى في نفس المكان والوقت.");

    //            var Class = await _classRepository.GetByIdAsync(id)
    //           ?? throw new KeyNotFoundException($"لم يتم العثور على المحاضرة برقم {id}.");

    //            bool isAlreadyAssigned = await _classRepository.IsCourseAlreadyAssignedAsync(updateClassDto.CourseId, updateClassDto.ProfessorId);
    //            if (isAlreadyAssigned)
    //            {
    //                throw new InvalidOperationException($"الكورس موجود بالفعل.");
    //            }

    //            // إنشاء الفصل
    //            var courseUpdate = _mapper.Map(updateClassDto, Class);
    //            await _classRepository.UpdateAsync(id, courseUpdate);
            
           
    //    }

    //    public async Task DeleteClassAsync(int id)
    //    {
    //        if (id <= 0)
    //            throw new ArgumentException("يجب أن يكون معرف الطالب رقمًا موجبا");

    //        var student = await _classRepository.GetByIdAsync(id) ??
    //            throw new KeyNotFoundException("لم يتم العثور على المحاضرة"); ;

    //        await _classRepository.DeleteAsync(id);
    //    }
    //    public async Task<int> GetClassCountAsync()
    //    {
    //        int count = await _classRepository.CountAsync();
    //        if (count == 0)
    //            throw new Exception("لا يوجد كورسات.");

    //        return count;
    //    }

    //    public async Task<IEnumerable<string>> GetAllLocationsNameAsync()
    //    {
    //        var names = await _classRepository.GetAllLocationsNameAsync();

    //        if (names == null || !names.Any())
    //            throw new Exception("لا يوجد أي قاعات");

    //        return names;
    //    }
    //}
}
