using SqlUniversity.DataAccess.Models;

namespace SqlUniversity.Model.Dtos
{
    public class CourseRuleDto
    {
        public int Id { get; set; }
        public int Year { get; set; }
        public int RequiredPoints { get; set; }

    }
}
