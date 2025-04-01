using FacultyManagementSystemAPI.Models.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace FacultyManagementSystemAPI.Data
{
    public class AppDbContext(DbContextOptions<AppDbContext> options) : IdentityDbContext<ApplicationUser>(options)
    {
        public DbSet<Student> Students { get; set; }
        public DbSet<Department> Departments { get; set; }
        public DbSet<Course> Courses { get; set; }
        public DbSet<Enrollment> Enrollments { get; set; }
        public DbSet<CourseDepartment> CourseDepartments { get; set; }
        public DbSet<Professor> Professors { get; set; }
        public DbSet<Class> Classes { get; set; }
        public DbSet<Attendance> Attendances { get; set; }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Seed Roles

            var studentId = "0c20a355-12dd-449d-a8d5-6e33960c45ee";
            var professorId = "393f1091-b175-4cca-a1df-19971e86e2a3";
            var adminId = "7d090697-295a-43bf-bb0b-3a19843fb528";
            var superAdminId = "8b2fbfe2-0a51-4f8e-b57f-4504d20a3739";

            var roles = new List<IdentityRole>
         {
             new IdentityRole
             {
                 Id = studentId,
                 ConcurrencyStamp =studentId,
                 Name = "Student",
                 NormalizedName = "Student".ToUpper()
             },
             new IdentityRole
             {
                 Id = professorId,
                 ConcurrencyStamp =professorId,
                 Name = "Professor",
                 NormalizedName = "Professor".ToUpper()
             },
             new IdentityRole
             {
                 Id = adminId,
                 ConcurrencyStamp =adminId,
                 Name = "Admin",
                 NormalizedName = "Admin".ToUpper()
             },
             new IdentityRole
             {
                 Id = superAdminId,
                 ConcurrencyStamp =superAdminId,
                 Name = "SuperAdmin",
                 NormalizedName = "SuperAdmin".ToUpper()
             }
                    };

            modelBuilder.Entity<ApplicationUser>(entity =>
            {
                entity.Property(e => e.UserName)
                      .HasColumnType("nvarchar(256)"); // استخدم NVARCHAR للأحرف العربية
            });

            modelBuilder.Entity<IdentityRole>().HasData(roles);


            // تخصيص Sequence واحدة لجميع الـ IDs
            modelBuilder.HasSequence<int>("CommonSequence", schema: "dbo")
                .StartsAt(1000)
                .IncrementsBy(5); // 5 الزيادة بمقدار 

            // Id start with 1000 and increase with 5 for all Ids
            modelBuilder.Entity<Attendance>()
                .Property(p => p.Id)
                .ValueGeneratedOnAdd()
                .HasDefaultValueSql("NEXT VALUE FOR dbo.CommonSequence");


            modelBuilder.Entity<Class>()
                .Property(p => p.Id)
                .ValueGeneratedOnAdd()
                .HasDefaultValueSql("NEXT VALUE FOR dbo.CommonSequence");

            modelBuilder.Entity<Course>()
                .Property(p => p.Id)
                .ValueGeneratedOnAdd()
                .HasDefaultValueSql("NEXT VALUE FOR dbo.CommonSequence");

            modelBuilder.Entity<Department>()
                .Property(p => p.Id)
                .ValueGeneratedOnAdd()
                .HasDefaultValueSql("NEXT VALUE FOR dbo.CommonSequence");

            modelBuilder.Entity<CourseDepartment>()
                .Property(p => p.Id)
                .ValueGeneratedOnAdd()
                .HasDefaultValueSql("NEXT VALUE FOR dbo.CommonSequence");

            modelBuilder.Entity<Enrollment>()
                .Property(p => p.Id)
                .ValueGeneratedOnAdd()
                .HasDefaultValueSql("NEXT VALUE FOR dbo.CommonSequence");

            modelBuilder.Entity<Professor>()
                .Property(p => p.Id)
                .ValueGeneratedOnAdd()
                .HasDefaultValueSql("NEXT VALUE FOR dbo.CommonSequence");

            modelBuilder.Entity<Student>()
                .Property(p => p.Id)
                .ValueGeneratedOnAdd()
                .HasDefaultValueSql("NEXT VALUE FOR dbo.CommonSequence");


            // Relationships

            modelBuilder.Entity<Student>()
           .HasOne(s => s.ApplicationUser)
           .WithOne(u => u.Student)
           .HasForeignKey<ApplicationUser>(u => u.StudentId)
           .OnDelete(DeleteBehavior.Cascade); // حذف المستخدم عند حذف الطالب

            // العلاقة بين Professor و ApplicationUser (واحد إلى واحد)
            modelBuilder.Entity<Professor>()
                .HasOne(p => p.ApplicationUser)
                .WithOne(u => u.Professor)
                .HasForeignKey<ApplicationUser>(u => u.ProfessorId)
                .OnDelete(DeleteBehavior.Cascade); // حذف المستخدم عند حذف الأستاذ


            // Enrollment With Student (1-M)
            modelBuilder.Entity<Enrollment>()
                .HasOne(e => e.Student)
                .WithMany(s => s.Enrollments)
                .HasForeignKey(e => e.StudentId)

                .OnDelete(DeleteBehavior.Cascade); // If a student is deleted, delete enrollments

            // Enrollment With Course (1-M)
            modelBuilder.Entity<Enrollment>()
                .HasOne(e => e.Course)
                .WithMany(c => c.Enrollments)
                .HasForeignKey(e => e.CourseId)
                .OnDelete(DeleteBehavior.Cascade); // If a course is deleted, delete enrollments

            // Course With PreCourse
            modelBuilder.Entity<Course>()
                .HasOne(c => c.PreCourse)
                .WithOne()
                .HasForeignKey<Course>(c => c.PreCourseId)
                .OnDelete(DeleteBehavior.NoAction);


            // Student With Department (1-M)
            modelBuilder.Entity<Student>()
                .HasOne(s => s.Department)
                .WithMany(d => d.Students)
                .HasForeignKey(s => s.DepartmentId)
                .OnDelete(DeleteBehavior.Restrict); // Prevent deletion if students exist		

            // DepartmentCourse With Department (1-M)
            modelBuilder.Entity<CourseDepartment>()
                .HasOne(dc => dc.Department)
                .WithMany(d => d.CourseDepartments)
                .HasForeignKey(dc => dc.DepartmentId)
                .OnDelete(DeleteBehavior.Cascade); // If a department is deleted, remove department courses

            // DepartmentCourse With Course (1-M)
            modelBuilder.Entity<CourseDepartment>()
                .HasOne(dc => dc.Course)
                .WithMany(c => c.CourseDepartments)
                .HasForeignKey(dc => dc.CourseId)
                .OnDelete(DeleteBehavior.Cascade); // If a course is deleted, remove department courses

            // ClassCourse With Course (1-M)
            modelBuilder.Entity<Class>()
                .HasOne(cc => cc.Course)
                .WithMany(c => c.Classes)
                .HasForeignKey(cc => cc.CourseId)
                .OnDelete(DeleteBehavior.Cascade); // If a course is deleted, remove class courses		

            // Attendance And Student_Schedule (1-M)
            modelBuilder.Entity<Attendance>()
                .HasOne(a => a.Class)
                .WithMany(c => c.Attendances)
                .HasForeignKey(a => a.ClassesId)
                .OnDelete(DeleteBehavior.Restrict); // Prevent deletion if attendance records exist

            // Attendance And Student (1-M)
            modelBuilder.Entity<Attendance>()
                .HasOne(a => a.Student)
                .WithMany(c => c.Attendances)
                .HasForeignKey(a => a.StudentId)
                .OnDelete(DeleteBehavior.Restrict); // Prevent deletion if attendance records exist

            // Professor And Department (1-M)
            modelBuilder.Entity<Professor>()
                .HasOne(f => f.Department)
                .WithMany(d => d.Professors)
                .HasForeignKey(f => f.DepartmentId)
                .OnDelete(DeleteBehavior.Restrict); // Prevent deletion if faculties exist

            // Class And Faculty (TeachClass) (1-M)
            modelBuilder.Entity<Class>()
                .HasOne(c => c.Professor)
                .WithMany(f => f.Classes)
                .HasForeignKey(c => c.ProfessorId)
                .OnDelete(DeleteBehavior.Restrict); // Prevent deletion of faculty if classes exist		
        }
    }
}
