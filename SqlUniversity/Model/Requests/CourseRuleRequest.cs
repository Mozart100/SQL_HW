using SqlUniversity.DataAccess.Models;

namespace SqlUniversity.Model.Requests
{
    public class CourseRuleRequest : ICourseRuleMapper
    {
        public int Year { get; set; }
        public int RequiredPoints { get; set; }
    }


    public class CourseRuleResponse : NetBetReponseBase<CourseRuleRequest>
    {
    }
}
