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
        public DbSet<Professor> Professors { get; set; }
        public DbSet<Class> Classes { get; set; }
        public DbSet<Attendance> Attendances { get; set; }
        public DbSet<Division> Divisions { get; set; }
        public DbSet<CourseDivision> CourseDivisions { get; set; }
        public DbSet<CoursePrerequisite> CoursePrerequisites { get; set; }
        public DbSet<AcademicWarning> AcademicWarnings { get; set; }
        public DbSet<EnrollmentPeriod> EnrollmentPeriods { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // إعدادات Sequence
            modelBuilder.HasSequence<int>("CommonSequence", schema: "dbo")
                .StartsAt(1000)
                .IncrementsBy(5);

            // تعيين Sequence لجميع الجداول
            var entityTypes = new List<Type>
        {
            typeof(Attendance), typeof(Class), typeof(Course), typeof(Department),
            typeof(Enrollment), typeof(Professor), typeof(Student), typeof(Division),
            typeof(CourseDivision), typeof(CoursePrerequisite)
        };

            foreach (var entityType in entityTypes)
            {
                modelBuilder.Entity(entityType)
                    .Property("Id")
                    .ValueGeneratedOnAdd()
                    .HasDefaultValueSql("NEXT VALUE FOR dbo.CommonSequence");
            }

            // إعدادات العلاقات
            modelBuilder.Entity<Student>()
                .HasOne(s => s.ApplicationUser)
                .WithOne(u => u.Student)
                .HasForeignKey<ApplicationUser>(u => u.StudentId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Professor>()
                .HasOne(p => p.ApplicationUser)
                .WithOne(u => u.Professor)
                .HasForeignKey<ApplicationUser>(u => u.ProfessorId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Enrollment>()
                .HasOne(e => e.Student)
                .WithMany(s => s.Enrollments)
                .HasForeignKey(e => e.StudentId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Enrollment>()
                .HasOne(e => e.Course)
                .WithMany(c => c.Enrollments)
                .HasForeignKey(e => e.CourseId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Student>()
                .HasOne(s => s.Division)
                .WithMany(d => d.Students)
                .HasForeignKey(s => s.DivisionId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Division>()
                .HasOne(d => d.Department)
                .WithMany(d => d.Divisions)
                .HasForeignKey(d => d.DepartmentId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Course>()
                .HasOne(c => c.Department)
                .WithMany(d => d.Courses)
                .HasForeignKey(c => c.DepartmentId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<CoursePrerequisite>()
                .HasOne(cp => cp.Course)
                .WithMany(c => c.Prerequisites)
                .HasForeignKey(cp => cp.CourseId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<CoursePrerequisite>()
                .HasOne(cp => cp.PrerequisiteCourse)
                .WithMany(c => c.IsPrerequisiteFor)
                .HasForeignKey(cp => cp.PrerequisiteCourseId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<CourseDivision>()
                .HasOne(cd => cd.Course)
                .WithMany(c => c.CourseDivisions)
                .HasForeignKey(cd => cd.CourseId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<CourseDivision>()
                .HasOne(cd => cd.Division)
                .WithMany(d => d.CourseDivisions)
                .HasForeignKey(cd => cd.DivisionId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Professor>()
                .HasOne(p => p.Department)
                .WithMany(d => d.Professors)
                .HasForeignKey(p => p.DepartmentId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Class>()
                .HasOne(c => c.Professor)
                .WithMany(p => p.Classes)
                .HasForeignKey(c => c.ProfessorId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Class>()
                .HasOne(c => c.Course)
                .WithMany(c => c.Classes)
                .HasForeignKey(c => c.CourseId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Attendance>()
                .HasOne(a => a.Class)
                .WithMany(c => c.Attendances)
                .HasForeignKey(a => a.ClassesId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Attendance>()
                .HasOne(a => a.Student)
                .WithMany(s => s.Attendances)
                .HasForeignKey(a => a.StudentId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<AcademicWarning>(entity =>
            {
                entity.HasOne(a => a.Student)
                      .WithMany(s => s.AcademicWarnings)
                      .HasForeignKey(a => a.StudentId)
                      .OnDelete(DeleteBehavior.Restrict);

                entity.HasIndex(a => a.StudentId); // إضافة index لتحسين الأداء
            });

            // Seed Roles
            var studentId = "0c20a355-12dd-449d-a8d5-6e33960c45ee";
            var professorId = "393f1091-b175-4cca-a1df-19971e86e2a3";
            var adminId = "7d090697-295a-43bf-bb0b-3a19843fb528";
            var superAdminId = "8b2fbfe2-0a51-4f8e-b57f-4504d20a3739";

            var roles = new List<IdentityRole>
        {
            new IdentityRole { Id = studentId, ConcurrencyStamp = studentId, Name = "Student", NormalizedName = "STUDENT" },
            new IdentityRole { Id = professorId, ConcurrencyStamp = professorId, Name = "Professor", NormalizedName = "PROFESSOR" },
            new IdentityRole { Id = adminId, ConcurrencyStamp = adminId, Name = "Admin", NormalizedName = "ADMIN" },
            new IdentityRole { Id = superAdminId, ConcurrencyStamp = superAdminId, Name = "SuperAdmin", NormalizedName = "SUPERADMIN" }
        };

            modelBuilder.Entity<IdentityRole>().HasData(roles);
        }
    }
}
