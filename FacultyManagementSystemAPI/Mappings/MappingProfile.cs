
using AutoMapper;
using FacultyManagementSystemAPI.Models.DTOs.Courses;
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
			}
		}
	}
