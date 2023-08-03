using SqlUniversity.DataAccess.Models;

namespace SqlUniversity.Model.Requests
{
    public class CourseRuleRequest
    {
        public int Year { get; set; }
        public int RequiredPoints { get; set; }
    }


    public class CourseRuleResponse : UniversityReponseBase<CourseRuleRequest>
    {
    }
}
