using FacultyManagementSystemAPI.Models.Entities;
using Microsoft.EntityFrameworkCore;

namespace FacultyManagementSystemAPI.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

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
