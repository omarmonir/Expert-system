
using AutoMapper;
using FacultyManagementSystemAPI.Models.DTOs.Attendance;
using FacultyManagementSystemAPI.Models.DTOs.Classes;
using FacultyManagementSystemAPI.Models.DTOs.Courses;
using FacultyManagementSystemAPI.Models.DTOs.Department;
using FacultyManagementSystemAPI.Models.DTOs.Enrollment;
using FacultyManagementSystemAPI.Models.DTOs.professors;
using FacultyManagementSystemAPI.Models.DTOs.Student;
using FacultyManagementSystemAPI.Models.Entities;

namespace FacultyManagementSystemAPI.Mappings
{

    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<CreateStudentDto, Student>();
            CreateMap<UpdateStudentDto, Student>();
            CreateMap<Student, StudentDto>();

            CreateMap<CreateCourseDto, Course>();
            CreateMap<UpdateCourseDto, Course>();
            CreateMap<Course, CourseDto>();


            CreateMap<CreateAttendanceDto, Attendance>();
            CreateMap<UpdateAttendanceDto, Attendance>();
            CreateMap<Attendance, AttendanceDto>().ReverseMap();

            CreateMap<CreateProfessorDto, Professor>();
            CreateMap<UpdateProfessorDto, Professor>();
            CreateMap<Professor, ProfessorDto>().ReverseMap();

            CreateMap<UpdateHeadOfDepartmentDto, Department>();
            CreateMap<UpdateHeadOfDepartmentDto, Department>();
            CreateMap<Department, DepartmentDto>();


            CreateMap<EnrollmentDto, Enrollment>().ReverseMap();

            // تعيين UpdateGradeDto إلى Enrollment
            CreateMap<UpdateGradeDto, Enrollment>()
                .ForMember(dest => dest.FinalGrade, opt => opt.Ignore());

            CreateMap<Class, ClassDto>();
            CreateMap<CreateClassDto, Class>();
            CreateMap<UpdateClassDto, Class>();
        }
    }
}
