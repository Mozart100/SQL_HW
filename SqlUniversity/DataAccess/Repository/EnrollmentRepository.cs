using SqlUniversity.DataAccess.Models;

namespace SqlUniversity.DataAccess.Repository
{
    public interface IEnrollmentRepository : IRepositoryBase<Enrollment>
    {
        Enrollment UpdateCourses(int enrollmentId, int[] courses);
        Enrollment UpdateCoursesAndState(int enrollmentId , int [] courses, EnrollmentTypeState newState);
        Enrollment UpdateEnrollmentStatus(int enrollmentId, EnrollmentTypeState state);
    }

    public class EnrollmentRepository : RepositoryBase<Enrollment>, IEnrollmentRepository
    {
        private readonly ILogger<EnrollmentRepository> _logger;

        public EnrollmentRepository(ILogger<EnrollmentRepository> logger)
        {
            _logger = logger;
        }

        public Enrollment UpdateCourses(int enrollmentId, int[] courses)
        {
            foreach (var enrollment in Models.ToArray())
            {
                if (enrollment.Id == enrollmentId)
                {
                    enrollment.Courses = courses;
                    return enrollment;
                }
            }

            return null;
        }

        public Enrollment UpdateCoursesAndState(int enrollmentId, int[] courses, EnrollmentTypeState state)
        {
            foreach (var enrollment in Models.ToArray())
            {
                if (enrollment.Id == enrollmentId)
                {
                    enrollment.TypeState = state;
                    enrollment.Courses = courses;
                    return enrollment;
                }
            }

            return null;
        }

        public Enrollment UpdateEnrollmentStatus(int enrollmentId, EnrollmentTypeState state)
        {
            foreach (var enrollment in Models.ToArray())
            {
                if (enrollment.Id == enrollmentId)
                {
                    enrollment.TypeState= state;
                    return enrollment;
                }
            }

            return null;
        }
    }
}
