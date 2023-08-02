namespace SqlUniversity.DataAccess.Models
{
    /// <summary>
    /// This interface solely for AutoMapper
    /// </summary>
    public interface ICourseRuleMapper
    {
        int Year { get; set; }

        int RequiredPoints { get; set; }

    }

    /// <summary>
    /// For Dto Mapper.
    /// </summary>
    public interface ICourseRuleDtoMapper : ICourseRuleMapper
    {
        int Id { get; set; }
    }

    public class CourseRule : EntityDbBase, ICourseRuleDtoMapper, ICourseRuleMapper
    {
        public int Year { get; set; }
        public int RequiredPoints { get; set; }

    }



}
