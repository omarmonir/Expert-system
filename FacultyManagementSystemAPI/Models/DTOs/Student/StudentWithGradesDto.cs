public class StudentWithGradesDto
{
    public int Id { get; set; }

    public string Name { get; set; }

    public decimal GPA_Average { get; set; }

    public decimal? FinalGrade { get; set; }

    public decimal? Exam1Grade { get; set; }

    public decimal? Exam2Grade { get; set; }

    public decimal? Grade { get; set; }
}
