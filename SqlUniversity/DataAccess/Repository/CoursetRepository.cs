using SqlUniversity.DataAccess.Models;

namespace SqlUniversity.DataAccess.Repository
{
    public interface ICoursetRepository : IRepositoryBase<Course>
    {
    }

    public class CoursetRepository : RepositoryBase<Course>, ICoursetRepository
    {
        private readonly ILogger<CoursetRepository> _logger;

        public CoursetRepository(ILogger<CoursetRepository> logger)
        {
            _logger = logger;
        }
    }
}
