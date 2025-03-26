using FacultyManagementSystemAPI.Data;
using FacultyManagementSystemAPI.Models.Entities;
using FacultyManagementSystemAPI.Repositories;
using FacultyManagementSystemAPI.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

public class DepartmentRepository(AppDbContext dbContext) : GenericRepository<Department>(dbContext), IDepartmentRepository
{
    private readonly AppDbContext _dbContext = dbContext;

    public async Task UpdateProfessorCountAsync(int id, int professorCount)
    {
        var department = await _dbContext.Departments.FindAsync(id);

        department.ProfessorCount = professorCount;
        await _dbContext.SaveChangesAsync();
    }

    public async Task UpdateHeadOfDepartmentAsync(int id, string headOfDepartment)
    {
        var department = await _dbContext.Departments.FindAsync(id);

        department.HeadOfDepartment = headOfDepartment;
        await _dbContext.SaveChangesAsync();
    }

    public async Task<IEnumerable<string>> GetAllDepartmentNameAsync()
    {
        var names = await _dbContext.Departments
            .Select(d => d.Name)
            .ToListAsync();

        return names;
    }

    public async Task<int?> GetIdOfDepartmentByNameAsync(string departmentName)
    {
        return await _dbContext.Departments
            .Where(d => d.Name.Trim().ToLower() == departmentName.Trim().ToLower()) // إزالة المسافات والتأكد من تطابق الأحرف
            .Select(d => (int?)d.Id)
            .FirstOrDefaultAsync();
    }
}
