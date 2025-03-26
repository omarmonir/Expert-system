using AutoMapper;
using FacultyManagementSystemAPI.Data;
using FacultyManagementSystemAPI.Models.DTOs.Enrollment;
using FacultyManagementSystemAPI.Models.DTOs.Student;
using FacultyManagementSystemAPI.Models.Entities;
using FacultyManagementSystemAPI.Repositories.Interfaces;
using FacultyManagementSystemAPI.Services.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace FacultyManagementSystemAPI.Services.Implementes
{
    public class StudentService(IStudentRepository studentRepository, IFileService fileService, IMapper mapper,
        UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager, IEmailService emailService) : IStudentService
    {
        private readonly IStudentRepository _studentRepository = studentRepository;
        private readonly IFileService _fileService = fileService;
        private readonly IMapper _mapper = mapper;
        private readonly UserManager<ApplicationUser> _userManager = userManager;
        private readonly RoleManager<IdentityRole> _roleManager = roleManager;
        private readonly IEmailService _emailService = emailService;



        //public async Task AddAsync(CreateStudentDto createStudentDto)
        //{
        //    if (createStudentDto == null)
        //        throw new ArgumentNullException(nameof(createStudentDto), "البيانات المدخلة لا يمكن أن تكون فارغة");

        //    await ValidateStudentData(createStudentDto);

        //    var student = _mapper.Map<Student>(createStudentDto);

        //    await _studentRepository.AddAsync(student);

        //    var existingUser = await _userManager.FindByEmailAsync(createStudentDto.Email);
        //    if (existingUser != null)
        //        throw new Exception("A user with this email already exists");

        //    var password = GenerateRandomPassword();

        //    var user = new ApplicationUser
        //    {
        //        UserName = createStudentDto.Email,
        //        Email = createStudentDto.Email,
        //        StudentId = student.Id,
        //        Password = password,
        //        UserType = "Student"
        //    };
        //    //user.PasswordHash = _userManager.PasswordHasher.HashPassword(user, password);

        //    var result = await _userManager.CreateAsync(user, password);

        //    if (!result.Succeeded)
        //    {
        //        //throw new Exception("InValid Email Or Password!");
        //        var errors = string.Join(", ", result.Errors.Select(e => e.Description));
        //        throw new Exception($"User creation failed: {errors}");
        //    }

        //    await _userManager.AddToRoleAsync(user, "Student");

        //    // var student = _mapper.Map<Student>(createStudentDto);

        //    student.ApplicationUserId = user.Id;
        //    student.ImagePath = _fileService.SaveFile(createStudentDto.Image, "Students");

        //    //try
        //    //{
        //    //    await _studentRepository.AddAsync(student);
        //    //}
        //    //catch (DbUpdateException ex)
        //    //{
        //    //    throw new Exception($"فشل تحديث قاعدة البيانات: {ex.InnerException?.Message}");
        //    //}
        //    //catch (Exception ex)
        //    //{
        //    //    throw new Exception($"حدث خطأ غير متوقع: {ex.Message}");
        //    //}

        //}

        public async Task AddAsync(CreateStudentDto createStudentDto)
        {
            if (createStudentDto == null)
                throw new ArgumentNullException(nameof(createStudentDto), "البيانات المدخلة لا يمكن أن تكون فارغة");

            // التحقق مما إذا كان الطالب موجودًا مسبقًا عبر البريد الإلكتروني
            var existingUser = await _userManager.FindByEmailAsync(createStudentDto.Email);
            if (existingUser != null)
                throw new Exception("يوجد مستخدم بهذا البريد الإلكتروني بالفعل");

            // إنشاء كلمة مرور عشوائية
            var password = GenerateRandomPassword();

            // 1️⃣ إنشاء المستخدم أولاً
            var user = new ApplicationUser
            {
                UserName = createStudentDto.Email,
                Email = createStudentDto.Email,
                UserType = "Student",
                StudentId = null, // سيتم تحديثه لاحقًا
                IsActive = true, // الحساب مفعل افتراضيًا
                RefreshToken = null,
                RefreshTokenExpiryTime = null,
                LastLoginDate = null,
                LastLoginIp = null,
                LastLoginDevice = null,
                DeactivationDate = null,
               
            };

            var result = await _userManager.CreateAsync(user, password);
            if (!result.Succeeded)
            {
                var errors = string.Join(", ", result.Errors.Select(e => e.Description));
                throw new Exception($"فشل إنشاء المستخدم: {errors}");
            }

            await _userManager.AddToRoleAsync(user, "Student");

            // 2️⃣ إنشاء الطالب وربطه بالمستخدم
            var student = _mapper.Map<Student>(createStudentDto);
            student.ApplicationUserId = user.Id;
            student.ImagePath = _fileService.SaveFile(createStudentDto.Image, "Students");

            try
            {
                await _studentRepository.AddAsync(student);

                // 3️⃣ تحديث معرف الطالب في المستخدم بعد إضافته
                user.StudentId = student.Id;
                await _userManager.UpdateAsync(user);
            }
            catch (DbUpdateException ex)
            {
                // حذف المستخدم إذا فشلت إضافة الطالب
                await _userManager.DeleteAsync(user);
                throw new Exception($"فشل تحديث قاعدة البيانات: {ex.InnerException?.Message}");
            }
            catch (Exception ex)
            {
                // حذف المستخدم إذا فشلت إضافة الطالب
                await _userManager.DeleteAsync(user);
                throw new Exception($"حدث خطأ غير متوقع: {ex.Message}");
            }

            // 4️⃣ إرسال البريد الإلكتروني للمستخدم الجديد
            string subject = "تفاصيل حسابك";

            string body = $@"
                        <h2>مرحبًا بك في نظام إدارة الكلية</h2>
                        <p style='font-family: Arial, sans-serif; color: #333;'>عزيزي {createStudentDto.Name}،</p>
                        <p style='font-family: Arial, sans-serif; color: #333;'>تم إنشاء حسابك بنجاح. فيما يلي بيانات تسجيل الدخول الخاصة بك:</p>

                        <div style='background-color: #f4f4f9; padding: 15px; border-radius: 8px; margin-bottom: 10px;'>
                            <p style='font-family: Arial, sans-serif; color: #333;'><strong>البريد الإلكتروني:</strong> {createStudentDto.Email}</p>
                            <p style='font-family: Arial, sans-serif; color: #333;'><strong>كلمة المرور:</strong> {password}</p>
                        </div>

                        <div style='background-color: #fff8e1; padding: 15px; border-radius: 8px; margin-bottom: 10px;'>
                            <h3 style='color: #d32f2f;'>إرشادات أمنية:</h3>
                            <ul>
          
                                <li>لا تشارك بيانات الدخول مع أي شخص</li>
                                <li>ستتلقى إشعارات عند تسجيل الدخول من أجهزة جديدة</li>
                            </ul>
                        </div>

                        <p style='font-family: Arial, sans-serif; color: #e74c3c; font-weight: bold;'>⚠ يرجى الاحتفاظ بهذه المعلومات بشكل آمن.</p>
                        <p style='font-family: Arial, sans-serif; color: #333;'>تحياتنا،</p>
                        <p style='font-family: Arial, sans-serif; color: #333;'><strong>إدارة النظام</strong></p>";

            try
            {
                bool emailSent = await _emailService.SendEmailAsync(user.Email, subject, body);
                if (!emailSent)
                {
                    throw new Exception("تم إنشاء الحساب بنجاح، ولكن فشل إرسال البريد الإلكتروني.");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("❌ خطأ في إرسال البريد: " + ex.Message);
                // يمكنك اختيارياً تسجيل الخطأ في نظام التسجيل (Logging) هنا
            }
        }



        public async Task<IEnumerable<StudentDto>> GetAllWithDepartmentNameAsync()
        {
            var studentsDto = await _studentRepository.GetAllWithDepartmentNameAsync();
            if (studentsDto == null || !studentsDto.Any())
                throw new Exception("لا يوجد طلاب");

            return studentsDto;
        }

        public async Task DeleteAsync(int id)
        {
            if (id <= 0)
                throw new ArgumentException("يجب أن يكون معرف الطالب رقمًا موجبا");

            var student = await _studentRepository.GetByIdAsync(id) ??
                throw new KeyNotFoundException("لم يتم العثور على الطالب"); ;

            // حذف الصورة إذا كانت موجودة
            //if (!string.IsNullOrEmpty(student.ImagePath))
            //{
            //	await _fileService.DeleteFileAsync(student.ImagePath);
            //}

            // حذف الطالب من قاعدة البيانات
            await _studentRepository.DeleteAsync(id);
        }

        public async Task<StudentDto> GetByIdWithDepartmentNameAsync(int id)
        {
            if (id <= 0)
                throw new ArgumentException("يجب أن يكون معرف الطالب رقمًا موجبا");

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

            if (id <= 0)
                throw new ArgumentException("يجب أن يكون معرف الطالب رقمًا موجبا");

            var existingStudent = await _studentRepository.GetByIdAsync(id) ??
                throw new KeyNotFoundException($"لم يتم العثور على الطالب بالرقم التعريفي {id}");

            //var createdStudent = _mapper.Map<CreateStudentDto>(updateStudentDto);
            //await ValidateStudentData(createdStudent);

            _mapper.Map(updateStudentDto, existingStudent);

            //// تحديث الصورة إذا تم تحميل صورة جديدة
            //if (updateStudentDto. != null)
            //{
            //    // حذف الصورة القديمة إذا كانت موجودة
            //    if (!string.IsNullOrEmpty(existingStudent.ImagePath))
            //    {
            //        await _fileService.DeleteFileAsync(existingStudent.ImagePath);
            //    }

            //    // حفظ الصورة الجديدة
            //    existingStudent.ImagePath = _fileService.SaveFile(updateStudentDto.Image, "Students");
            //}

            await _studentRepository.UpdateAsync(id, existingStudent);
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
            if (id <= 0)
                throw new ArgumentException("يجب أن يكون معرف الطالب رقمًا موجبا");

            var gradesDto = await _studentRepository.GetByIdWithHisGradeAsync(id) ??
                 throw new Exception("الطالب غير موجود");

            return gradesDto;
        }

        public async Task<IEnumerable<StudentCountDto>> GetStudentCountByDepartmentAsync(int departmentId)
        {
            if (departmentId <= 0)
                throw new ArgumentException("يجب أن يكون معرف القسم رقمًا موجبا");

            var studentCounts = await _studentRepository.GetStudentCountByDepartmentAsync(departmentId);

            if (studentCounts == null || !studentCounts.Any())
            {
                throw new Exception($"لم يتم العثور على طلاب لهذا القسم رقم {departmentId}");
            }

            return studentCounts;
        }

        public async Task<IEnumerable<StudentDto>> GetUnenrolledStudentsAsync()
        {
            var studentsDto = await _studentRepository.GetUnenrolledStudentsAsync();
            if (studentsDto == null || !studentsDto.Any())
                throw new Exception("لا يوجد طلاب غير مسجلين");

            return studentsDto;
        }

        public async Task<IEnumerable<StudentDto>> GetUnenrolledStudentsByDepartmentAsync(int departmentId)
        {
            if (departmentId <= 0)
                throw new ArgumentException("يجب أن يكون معرف القسم رقمًا موجبا");

            if (!await _studentRepository.DepartmentExistsAsync(departmentId))
            {
                throw new KeyNotFoundException("لم يتم العثور على القسم");
            }

            var students = await _studentRepository.GetUnenrolledStudentsByDepartmentAsync(departmentId);

            if (!students.Any())
                throw new KeyNotFoundException("لا يوجد طلاب غير مسجلين في هذا القسم");

            return students;
        }

        public async Task<IEnumerable<StudentDto>> GetUnenrolledStudentsBySemesterAsync(byte semester)
        {
            if (semester <= 0)
                throw new ArgumentException("يجب أن يكون معرف الفصل رقمًا موجبا");

            if (semester > 8)
                throw new ArgumentException("رقم الفصل الدراسي يجب ألا يتعدى 8");

            var students = await _studentRepository.GetUnenrolledStudentsBySemesterAsync(semester);

            if (!students.Any())
                throw new KeyNotFoundException("لا يوجد طلاب غير مسجلين في أي كورس لهذا الفصل الدراسي");

            return students;
        }

        public async Task<IEnumerable<EnrollmentDto>> GetEnrollmentsByDateRangeAsync(DateTime? minDate, DateTime? maxDate)
        {
            if (minDate.HasValue && maxDate.HasValue && minDate > maxDate)
                throw new ArgumentException("التاريخ الأدنى لا يمكن أن يكون بعد التاريخ الأعلى");

            var enrollments = await _studentRepository.GetEnrollmentsByDateRangeAsync(minDate, maxDate);

            if (!enrollments.Any())
                throw new KeyNotFoundException("لم يتم العثور على أي تسجيلات ضمن هذا النطاق الزمني");

            return enrollments;
        }

        public async Task<IEnumerable<StudentDto>> GetFilteredStudentsAsync(StudentFilterDto filter)
        {

            var query = await _studentRepository.GetFilteredStudentsAsync(filter);

            if (filter.MinHigh_School_degree.HasValue && filter.MaxHigh_School_degree.HasValue)
            {
                if (filter.MinHigh_School_degree > filter.MaxHigh_School_degree)
                    throw new ArgumentException("الحد الأدنى لمجموع الثانوية العامة يجب أن يكون أقل من الحد الأقصى.");
                query = query.Where(s => s.High_School_degree >= filter.MinHigh_School_degree.Value &&
                                         s.High_School_degree <= filter.MaxHigh_School_degree.Value);
            }

            if (filter.MinEnrollmentDate.HasValue && filter.MaxEnrollmentDate.HasValue)
            {
                if (filter.MinEnrollmentDate > filter.MaxEnrollmentDate)
                    throw new ArgumentException("تاريخ الالتحاق الأدنى يجب أن يكون قبل تاريخ الالتحاق الأقصى.");

                query = query.Where(s => s.EnrollmentDate >= filter.MinEnrollmentDate.Value &&
                                         s.EnrollmentDate <= filter.MaxEnrollmentDate.Value);
            }

            if (filter.MinDateOfBirth.HasValue && filter.MaxDateOfBirth.HasValue)
            {
                if (filter.MinDateOfBirth > filter.MaxDateOfBirth)
                    throw new ArgumentException("تاريخ الميلاد الأدنى يجب أن يكون قبل تاريخ الميلاد الأقصى.");

                query = query.Where(s => s.DateOfBirth >= filter.MinDateOfBirth.Value &&
                                         s.DateOfBirth <= filter.MaxDateOfBirth.Value);
            }

            if (filter.MinGPA.HasValue && filter.MaxGPA.HasValue)
            {
                if (filter.MinGPA > filter.MaxGPA)
                    throw new ArgumentException("المعدل التراكمي الأدنى يجب أن يكون أقل من المعدل التراكمي الأقصى.");

                query = query.Where(s => s.GPA_Average >= filter.MinGPA.Value && s.GPA_Average <= filter.MaxGPA.Value);
            }

            if (query == null || !query.Any())
                throw new Exception("لا يوجد طلاب");

            return query;
        }

        public async Task<IEnumerable<StudentDto>> GetStudentsByDepartmentAndNameAsync(string? departmentName, string? studentName, string? studentStatus)
        {
            var studentListDto = await _studentRepository.GetStudentsByDepartmentAndNameAsync(departmentName, studentName, studentStatus);

            if (studentListDto == null || !studentListDto.Any())
                throw new Exception("لا يوجد طلاب");

            return studentListDto;
        }

        public async Task<IEnumerable<StudentDto>> GetAllByDepartmentIdAsync(int departmentId)
        {
            if (departmentId <= 0)
                throw new ArgumentException("يجب أن يكون معرف القسم رقمًا موجبا");

            if (!await _studentRepository.DepartmentExistsAsync(departmentId))
                throw new KeyNotFoundException("لم يتم العثور على القسم");

            var students = await _studentRepository.GetAllByDepartmentIdAsync(departmentId);

            if (students == null || !students.Any())
                throw new Exception("لا يوجد طلاب");

            return students;
        }

        public async Task<IEnumerable<StudentDto>> GetStudentsByDepartmentNameAsync(string departmentName)
        {
            var students = await _studentRepository.GetStudentsByDepartmentNameAsync(departmentName);

            if (students == null || !students.Any())
                throw new Exception("لا يوجد طلاب فى هذا القسم");

            var studentListDto = _mapper.Map<IEnumerable<StudentDto>>(students);
            return studentListDto;
        }

        public async Task<int> GetStudentCountAsync()
        {
            int count = await _studentRepository.CountAsync();
            if (count == 0)
                throw new Exception("لا يوجد طلاب.");

            return count;
        }

        public async Task<int> CountCanceledEnrolledStudentsAsync()
        {
            int count = await _studentRepository.CountCanceledEnrolledStudentsAsync();
            if (count == 0)
                throw new Exception("لا طلاب غير مسجلين");

            return count;
        }
        public async Task<int> GetAllEnrollmentStudentsCountAsync()
        {
            int count = await _studentRepository.GetAllEnrollmentStudentsCountAsync();
            if (count == 0)
                throw new Exception("لا طلاب مسجلين");
            return count;

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

        public async Task<IEnumerable<string>> GetAllStudentStatusesAsync()
        {
            var status = await _studentRepository.GetAllStudentStatusesAsync();

            if (status == null || !status.Any())
                throw new Exception("لا يوجد أي حالات للطلاب");

            return status;
        }

        public async Task<IEnumerable<string>> GetAllStudentLevelsAsync()
        {
            var studentLevel = await _studentRepository.GetAllStudentLevelsAsync();

            if (studentLevel == null || !studentLevel.Any())
                throw new Exception("لا يوجد أي مستوى للطلاب");

            return studentLevel;
        }

        public async Task<IEnumerable<string>> GetAllStudentGenderAsync()
        {
            var genders = await _studentRepository.GetAllStudentGenderAsync();

            if (genders == null || !genders.Any())
                throw new Exception("لا يوجد أي أنواع");

            return genders;
        }

        public async Task<IEnumerable<StudentExamGradesDto>> GetStudentsWithExamGradesByCourseIdAsync(int courseId)
        {
            var students = await _studentRepository.GetStudentsWithExamGradesByCourseIdAsync(courseId);

            if (students == null || !students.Any())
                throw new Exception("لا يوجد طلاب مسجلين في هذا الكورس");

            return _mapper.Map<IEnumerable<StudentExamGradesDto>>(students);
        }

        public async Task<int> CountEnrollmentCoursesByStudentIdAsync(int studentId)
        {
            int count = await _studentRepository.CountEnrollmentCoursesByStudentIdAsync(studentId);
            if (count == 0)
                throw new Exception("لا يوجد تسجيلات لهذا الطالب");

            return count;
        }

        public async Task<int> CountCompletedCoursesCountStudentIdAsync(int studentId)
        {
            int count = await _studentRepository.CountCompletedCoursesCountStudentIdAsync(studentId);
            if (count == 0)
                throw new Exception("لا يوجد تسجيلات مكتملة لهذا الطالب");

            return count;
        }

        public async Task UpdateStudentStatusAsync(int studentId, string newStatus)
        {
            if (studentId <= 0)
                throw new ArgumentException("معرف الطالب غير صالح");

            var student = await _studentRepository.GetByIdAsync(studentId) ??
                throw new Exception(" الطالب غير موجود");

            if (student.status == newStatus)
                throw new InvalidOperationException("حالة الطالب الحالية مطابقة للحالة الجديدة، لا يوجد تغيير مطلوب");

            await _studentRepository.UpdateStudentStatusAsync(studentId, newStatus);
        }

        public async Task<int> CountStudentsByCourseIdAsync(int courseId)
        {
            var studentsCount = await _studentRepository.CountStudentsByCourseIdAsync(courseId);

            if (studentsCount == 0)
                throw new Exception("لا يوجد طلاب مسجلين في هذا الكورس");
            return studentsCount;
        }

        private string GenerateRandomPassword()
        {
            const string upperCase = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
            const string lowerCase = "abcdefghijklmnopqrstuvwxyz";
            const string digits = "0123456789";
            const string allChars = upperCase + lowerCase + digits;

            var random = new Random();

            // ضمان وجود حرف كبير، حرف صغير، ورقم
            string password =
                upperCase[random.Next(upperCase.Length)].ToString() +
                lowerCase[random.Next(lowerCase.Length)].ToString() +
                digits[random.Next(digits.Length)].ToString();

            // إكمال كلمة المرور بالأحرف العشوائية حتى تصل للطول المطلوب (8)
            password += new string(Enumerable.Repeat(allChars, 5) // 5 لأن لدينا 3 أحرف مضافة مسبقًا
                .Select(s => s[random.Next(s.Length)]).ToArray());

            // خلط الأحرف حتى لا يكون النمط ثابتًا
            return new string(password.OrderBy(_ => random.Next()).ToArray());
        }

    }
}