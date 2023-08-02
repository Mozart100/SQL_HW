using SqlUniversity.DataAccess.Models;

namespace SqlUniversity.DataAccess.Repository
{
    public interface ICourseRulesRepository : IRepositoryBase<CourseRule>
    {
    }

    public class CourseRulesRepository : RepositoryBase<CourseRule>, ICourseRulesRepository
    {
    }

}
