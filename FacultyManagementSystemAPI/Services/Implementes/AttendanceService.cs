using AutoMapper;
using FacultyManagementSystemAPI.Models.DTOs.Attendance;
using FacultyManagementSystemAPI.Models.Entities;
using FacultyManagementSystemAPI.Repositories.Interfaces;
using FacultyManagementSystemAPI.Services.Interfaces;

namespace FacultyManagementSystemAPI.Services.Implementes
{
    public class AttendanceService(IAttendanceRepository attendanceRepository, IMapper mapper) : IAttendanceService
    {
        private readonly IAttendanceRepository _attendanceRepository = attendanceRepository;
        private readonly IMapper _mapper = mapper;

        public async Task AddAttendanceAsync(CreateAttendanceDto CreateAttendanceDto)
        {
            if (CreateAttendanceDto == null)
                throw new ArgumentException("بيانات الحضور غير صالحة.");

            if (CreateAttendanceDto.Date > DateTime.Now)
                throw new ArgumentException("لا يمكن أن يكون تاريخ الحضور في المستقبل.");

            if (CreateAttendanceDto.StudentId <= 0 || CreateAttendanceDto.ClassesId <= 0)
                throw new ArgumentException("معرف الطالب أو معرف الفصل غير صالح.");

            var attendance = _mapper.Map<Attendance>(CreateAttendanceDto);

            await _attendanceRepository.AddAsync(attendance);
        }

        public async Task DeleteAttendanceAsync(int id)
        {
            if (id <= 0)
                throw new ArgumentException("يجب أن يكون معرف الحضور رقمًا موجبا");

            var attendance = await _attendanceRepository.GetAttendanceByIdAsync(id) ??
                throw new KeyNotFoundException("لم يتم العثور على سجل الحضور.");

            await _attendanceRepository.DeleteAsync(id);
        }

        public async Task<IEnumerable<AttendanceDto>> GetAllAttendancesAsync()
        {
            var attendanceListDto = await _attendanceRepository.GetAllAttendancesAsync();
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

        public async Task<IEnumerable<AttendanceDto>> GetAttendancesByClassIdAsync(int classId)
        {
            if (classId <= 0)
                throw new ArgumentException("يجب أن يكون معرف المحاضرة رقمًا موجبا");

            var attendanceListDto = await _attendanceRepository.GetAttendancesByClassIdAsync(classId);

            if (attendanceListDto == null || !attendanceListDto.Any())
                throw new Exception("لم يتم العثور على سجلات حضور.");

            return attendanceListDto; throw new NotImplementedException();
        }

        public async Task<IEnumerable<AttendanceDto>> GetAttendancesByStudentIdAsync(int studentId)
        {
            if (studentId <= 0)
                throw new ArgumentException("يجب أن يكون معرف الطالب رقمًا موجبا");

            var attendanceListDto = await _attendanceRepository.GetAttendancesByStudentIdAsync(studentId);
            if (attendanceListDto == null || !attendanceListDto.Any())
                throw new Exception("لم يتم العثور على سجلات حضور.");

            return attendanceListDto;
        }

        public async Task UpdateAttendanceAsync(int id, UpdateAttendanceDto updateAttendanceDto)
        {
            if (id <= 0)
                throw new ArgumentException("يجب أن يكون معرف الحضور رقمًا موجبا");

            var attendance = await _attendanceRepository.GetByIdAsync(id) ??
                throw new KeyNotFoundException("لم يتم العثور على سجل الحضور.");

            if (updateAttendanceDto == null)
                throw new ArgumentException("بيانات الحضور غير صالحة.");

            if (updateAttendanceDto.Date > DateTime.Now)
                throw new ArgumentException("لا يمكن أن يكون تاريخ الحضور في المستقبل.");

            if (updateAttendanceDto.StudentId <= 0 || updateAttendanceDto.ClassesId <= 0)
                throw new ArgumentException("معرف الطالب أو معرف الفصل غير صالح.");

            _mapper.Map(updateAttendanceDto, attendance);

            await _attendanceRepository.UpdateAsync(id, attendance);
        }
    }
}
