namespace SqlUniversity.Model.Dtos
{
    public class AcademicYearDto
    {
        public AcademicYearDto(int year, int requiredPoints, IEnumerable<int> mandatoryCourses)
        {
            Year = year;
            RequiredPoints = requiredPoints;
            MandatoryCourses = new List<int>(mandatoryCourses);
        }

        public int Year { get; }
        public int RequiredPoints { get; }
        public List<int> MandatoryCourses { get; }

    }
}
