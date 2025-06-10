namespace FacultyManagementSystemAPI.Models.DTOs
{
    public class PaginatedResult<T>
    {
        public IEnumerable<T> Items { get; set; }
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
    }

}
