using SqlUniversity.DataAccess.Models;

namespace SqlUniversity.Model.Dtos
{
    public class CourseRuleDto : ICourseRuleDtoMapper
    {
        public int Id { get; set; }
        public int Year { get; set; }
        public int RequiredPoints { get; set; }

    }
}
