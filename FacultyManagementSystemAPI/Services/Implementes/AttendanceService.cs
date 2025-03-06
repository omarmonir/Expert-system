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

        public async Task AddAttendanceAsync(CreateAttendanceDto createAttendanceDto)
        {
            try
            {
                if (createAttendanceDto == null)
                    throw new ArgumentException("بيانات الحضور غير صالحة.");

                if (createAttendanceDto.Date > DateTime.Now)
                    throw new ArgumentException("لا يمكن أن يكون تاريخ الحضور في المستقبل.");

                if (createAttendanceDto.StudentId <= 0 || createAttendanceDto.ClassesId <= 0)
                    throw new ArgumentException("معرف الطالب أو معرف الفصل غير صالح.");

                // التحقق من وجود الطالب
                var studentExists = await _attendanceRepository.StudentExistsAsync(createAttendanceDto.StudentId);
                if (!studentExists)
                    throw new Exception("الطالب غير موجود في قاعدة البيانات.");

                // التحقق من وجود الفصل
                var classExists = await _attendanceRepository.ClassExistsAsync(createAttendanceDto.ClassesId);
                if (!classExists)
                    throw new Exception("الفصل غير موجود في قاعدة البيانات.");

                var attendance = _mapper.Map<Attendance>(createAttendanceDto);

                await _attendanceRepository.AddAsync(attendance);
            }
            catch (DbUpdateException dbEx)
            {
                throw new Exception($"خطأ في قاعدة البيانات: {dbEx.InnerException?.Message}", dbEx);
            }
            catch (Exception ex)
            {
                throw new Exception($"حدث خطأ أثناء إضافة الحضور: {ex.Message}", ex);
            }
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
