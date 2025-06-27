﻿using FacultyManagementSystemAPI.Models.DTOs.Report;
using FacultyManagementSystemAPI.Repositories.Interfaces;
using FacultyManagementSystemAPI.Services.Interfaces;
using iTextSharp.text.pdf;
using iTextSharp.text;
using FacultyManagementSystemAPI.Models.DTOs.Courses;
using FacultyManagementSystemAPI.Repositories.Implementes;
using FacultyManagementSystemAPI.Models.DTOs.Classes;

namespace FacultyManagementSystemAPI.Services.Implementes
{
    public class ReportService : IReportService
    {
        private readonly IReportRepository _reportRepository;
        private readonly ExcelService _excelService;
        private readonly PdfService _pdfService;

        public ReportService(IReportRepository reportRepository, ExcelService excelService, PdfService pdfService)
        {
            _reportRepository = reportRepository;
            _excelService = excelService;
            _pdfService = pdfService;
        }
        public async Task<byte[]> ExportToExcelAsync<T>(IEnumerable<T> data, string sheetName)
        {
            return await _excelService.GenerateExcelAsync(data, sheetName);
        }

        public async Task<byte[]> ExportToPdfAsync<T>(IEnumerable<T> data, string title)
        {
            return await _pdfService.GeneratePdfAsync(data, title);
        }
        public async Task<byte[]> GenerateProfessorSchedulePdfAsync(IEnumerable<ClassDto> classes, string professorName)
        {
            return await _pdfService.GenerateProfessorSchedulePdfAsync(classes, professorName);
        }
        public async Task<IEnumerable<AcademicWarningDto>> GetAcademicWarningsAsync()
        {
            return await _reportRepository.GetAcademicWarningsAsync();
        }

        public Task<IEnumerable<CourseEnrollmentDto>> GetCourseEnrollmentTrendsAsync()
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<CourseGradeReportDto>> GetCourseGradesAsync()
        {
            throw new NotImplementedException();
        }

        public async Task<IEnumerable<ProfessorCoursesDto>> GetProfessorCoursesAsync()
        {
            return await _reportRepository.GetProfessorCoursesAsync();
        }

        public Task<IEnumerable<StudentAttendanceDto>> GetStudentAttendanceAsync()
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<StudentPerCourseDto>> GetStudentsPerCourseAsync()
        {
            throw new NotImplementedException();
        }


        //public async Task<IEnumerable<StudentPerDepartmentDto>> GetStudentsPerDepartmentAsync()
        //{
        //    return await _reportRepository.GetStudentsPerDepartmentAsync();
        //}
        public async Task<IEnumerable<FilterDto>> GetFilteredCoursesAsync(string? courseName, string? departmentName, string? courseStatus, string? divisionName)
        {
            var courses = await _reportRepository.GetFilteredCoursesAsync(courseName, departmentName, courseStatus, divisionName);

            if (!courses.Any())
                throw new KeyNotFoundException("لا يوجد كورسات مطابقة للمعايير المحددة.");

            return courses;
        }

        public async Task<byte[]> GenerateStudentSchedulePdfAsync(IEnumerable<ClassDto> classes, string StudentName)
        {
            return await _pdfService.GenerateStudentSchedulePdfAsync(classes, StudentName);
        }

        public async Task<byte[]> GenerateAdminClassesPdfAsync(IEnumerable<ClassDto> classes, string? filterInfo = null)
        {
            return await _pdfService.GenerateAdminClassesPdfAsync(classes, filterInfo);
        }
    }

}
