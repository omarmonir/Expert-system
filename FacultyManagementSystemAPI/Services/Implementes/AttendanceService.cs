using AutoMapper;
using FacultyManagementSystemAPI.Models.DTOs.Attendance;
using FacultyManagementSystemAPI.Models.Entities;
using FacultyManagementSystemAPI.Repositories.Interfaces;
using FacultyManagementSystemAPI.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace FacultyManagementSystemAPI.Services.Implementes
{
    public class AttendanceService(IAttendanceRepository attendanceRepository, IMapper mapper) : IAttendanceService
    {
        private readonly IAttendanceRepository _attendanceRepository = attendanceRepository;
        private readonly IMapper _mapper = mapper;

        public async Task AddAttendanceAsync(CreateAttendanceDto createAttendanceDto, int professorId)
        {
            if (createAttendanceDto == null)
                throw new ArgumentException("بيانات الحضور غير صالحة.");

            if (createAttendanceDto.Date > DateTime.Now)
                throw new ArgumentException("لا يمكن أن يكون تاريخ الحضور في المستقبل.");

            if (createAttendanceDto.StudentId <= 0)
                throw new ArgumentException("معرف الطالب غير صالح.");

            // تحقق من وجود الطالب
            var studentExists = await _attendanceRepository.StudentExistsAsync(createAttendanceDto.StudentId);
            if (!studentExists)
                throw new Exception("الطالب غير موجود.");

            // البحث عن الكورس
            var course = await _attendanceRepository.GetCourseByNameAsync(createAttendanceDto.CourseName);
            if (course == null)
                throw new Exception("الكورس غير موجود.");

            // البحث عن الفصل الذي ينتمي إليه الأستاذ والكورس معًا
            var classEntity = await _attendanceRepository.GetClassByProfessorAndCourseAsync(professorId, course.Id);
            if (classEntity == null)
                throw new Exception("هذا الأستاذ لا يدرس هذا الكورس.");

            // أنشئ الحضور
            var attendance = new Attendance
            {
                Date = createAttendanceDto.Date,
                Status = createAttendanceDto.Status,
                StudentId = createAttendanceDto.StudentId,
                ClassesId = classEntity.Id
            };

            await _attendanceRepository.AddAsync(attendance);
        }



        public async Task<int> CountAttendanceAsync()
        {
            int count = await _attendanceRepository.CountAttendanceAsync();
            if (count == 0)
                throw new Exception("لا يوجد حضور.");

            return count;
        }

        public async Task<int> CountNoAttendanceAsync()
        {
            int count = await _attendanceRepository.CountNoAttendanceAsync();
            if (count == 0)
                throw new Exception("لا يوجد غياب.");

            return count;
        }

        public async Task DeleteAttendanceAsync(int id)
        {
            if (id <= 0)
                throw new ArgumentException("يجب أن يكون معرف الحضور رقمًا موجبا");

            var attendance = await _attendanceRepository.GetAttendanceByIdAsync(id) ??
                throw new KeyNotFoundException("لم يتم العثور على سجل الحضور.");

            await _attendanceRepository.DeleteAsync(id);
        }

        public async Task<IEnumerable<AttendanceDto>> GetAllAttendancesAsync(int pageNumber)
        {
            var attendanceListDto = await _attendanceRepository.GetAllAttendancesAsync(pageNumber);
            if (attendanceListDto == null || !attendanceListDto.Any())
                throw new Exception("لم يتم العثور على سجلات حضور.");

            return attendanceListDto;
        }

        public async Task<AttendanceDto> GetAttendanceByIdAsync(int id)
        {
            if (id <= 0)
                throw new ArgumentException("يجب أن يكون معرف الحضور رقمًا موجبا");

            var attendance = await _attendanceRepository.GetAttendanceByIdAsync(id) ??
                throw new KeyNotFoundException("لم يتم العثور على سجل الحضور.");
            return attendance;
        }

        public async Task<IEnumerable<AttendanceDto>> GetAttendancesByClassIdAsync(int classId, int pageNumber)
        {
            if (classId <= 0)
                throw new ArgumentException("يجب أن يكون معرف المحاضرة رقمًا موجبا");

            var attendanceListDto = await _attendanceRepository.GetAttendancesByClassIdAsync(classId, pageNumber);

            if (attendanceListDto == null || !attendanceListDto.Any())
                throw new Exception("لم يتم العثور على سجلات حضور.");

            return attendanceListDto; throw new NotImplementedException();
        }

        public async Task<IEnumerable<AttendanceDto>> GetAttendancesByStudentIdAsync(int studentId, int pageNumber)
        {
            if (studentId <= 0)
                throw new ArgumentException("يجب أن يكون معرف الطالب رقمًا موجبا");

            var attendanceListDto = await _attendanceRepository.GetAttendancesByStudentIdAsync(studentId, pageNumber);
            if (attendanceListDto == null || !attendanceListDto.Any())
                throw new Exception("لا يوجد غياب لهذا الطالب.");

            return attendanceListDto;
        }

        public async Task<double> GetSuccessPercentageAsync()
        {
            int totalAttendances = await _attendanceRepository.CountAttendanceAsync();

            int totalNoAttendances = await _attendanceRepository.CountNoAttendanceAsync();

            double successPercentage = ((double)totalNoAttendances / totalAttendances) * 100;

            return Math.Round(successPercentage, 2);
        }

        public async Task UpdateAttendanceAsync(int id, UpdateAttendanceDto dto, int professorId)
        {
            if (id <= 0)
                throw new ArgumentException("يجب أن يكون معرف الحضور رقمًا موجبًا.");

            if (dto == null)
                throw new ArgumentException("بيانات الحضور غير صالحة.");

            if (dto.Date > DateTime.Now)
                throw new ArgumentException("لا يمكن أن يكون تاريخ الحضور في المستقبل.");

            var attendance = await _attendanceRepository.GetByIdAsync(id)
                ?? throw new KeyNotFoundException("لم يتم العثور على سجل الحضور.");

            // تحقق من وجود الطالب
            var studentExists = await _attendanceRepository.StudentExistsAsync(dto.StudentId);
            if (!studentExists)
                throw new Exception("الطالب غير موجود.");

            // تحقق من وجود الكورس
            var course = await _attendanceRepository.GetCourseByNameAsync(dto.CourseName);
            if (course == null)
                throw new Exception("الكورس غير موجود.");

            // تحقق من وجود الفصل المناسب لهذا الأستاذ والكورس
            var classEntity = await _attendanceRepository.GetClassByProfessorAndCourseAsync(professorId, course.Id);
            if (classEntity == null)
                throw new Exception("هذا الأستاذ لا يدرس هذا الكورس.");

            // تحديث بيانات الحضور
            attendance.Date = dto.Date;
            attendance.Status = dto.Status;
            attendance.StudentId = dto.StudentId;
            attendance.ClassesId = classEntity.Id;

            await _attendanceRepository.UpdateAsync(id, attendance);
        }

    }
}
