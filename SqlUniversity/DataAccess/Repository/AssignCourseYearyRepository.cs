using SqlUniversity.DataAccess.Models;

namespace SqlUniversity.DataAccess.Repository
{
    public interface IAssignCourseYearyRepository : IRepositoryBase<AssignCourseYearly>
    {

    }

    public class AssignCourseYearyRepository : RepositoryBase<AssignCourseYearly>, IAssignCourseYearyRepository
    {
      
    }
}
