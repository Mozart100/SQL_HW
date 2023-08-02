using AutoMapper;
using SqlUniversity.DataAccess.Models;
using SqlUniversity.DataAccess.Repository;
using SqlUniversity.Model.Dtos;
using SqlUniversity.Model.Requests;

namespace SqlUniversity.Services
{
    public interface ICourseService
    {
        CourseDto AddCourse(CourseRequest request);
        CourseRuleResponse AddCourseRule(CourseRuleRequest request);
        IEnumerable<CourseDto> GetAllCourse();
        IEnumerable<CourseRuleDto> GetAllCourseRules();
        CourseDto GetCourseById(int courseId);
    }
    public class CourseService : ICourseService
    {
        private readonly ILogger<CourseService> _logger;
        private readonly IMapper _mapper;
        private readonly ICoursetRepository _coursetRepository;
        private readonly IAssignCourseYearyRepository _assignCourseYearyRepository;
        private readonly ICourseRulesRepository _courseRulesRepository;

        public CourseService(ILogger<CourseService> logger,
            IMapper mapper,
            ICoursetRepository coursetRepository,
            IAssignCourseYearyRepository assignCourseYearyRepository,
            ICourseRulesRepository courseRulesRepository
            )
        {
            this._logger = logger;
            this._mapper = mapper;
            this._coursetRepository = coursetRepository;
            this._assignCourseYearyRepository = assignCourseYearyRepository;
            this._courseRulesRepository = courseRulesRepository;
        }

        public CourseDto AddCourse(CourseRequest request)
        {
            var course = _mapper.Map<Course>(request);

            var savedCoure = _coursetRepository.Insert(course);
            foreach (var yearSuited in request.YearsSuited)
            {
                var courseYearly = new AssignCourseYearly { CourseId = savedCoure.Id, Year = yearSuited };
                _assignCourseYearyRepository.Insert(courseYearly);
            }


            _logger.LogInformation("Course Added {course}", course.Id);
            return _mapper.Map<CourseDto>(savedCoure);
        }

        public CourseRuleResponse AddCourseRule(CourseRuleRequest request)
        {
            var course = _mapper.Map<CourseRule>(request);
            var savedCourse = _courseRulesRepository.Insert(course);
            var response = _mapper.Map<CourseRuleResponse>(savedCourse);
            response.IsOperationPassed = true;
            response.Request = request;
            return response;
        }

        public IEnumerable<CourseDto> GetAllCourse()
        {
            var courses = new List<CourseDto>();

            foreach (var course in _coursetRepository.GetAll())
            {
                courses.Add(_mapper.Map<CourseDto>(course));
            }

            return courses;
        }

        public IEnumerable<CourseRuleDto> GetAllCourseRules()
        {
            var courses = new List<CourseRuleDto>();

            foreach (var course in _courseRulesRepository.GetAll())
            {
                courses.Add(_mapper.Map<CourseRuleDto>(course));
            }

            return courses;
        }

        public CourseDto GetCourseById(int courseId)
        {
            var course = _coursetRepository.Get(x => x.Id == courseId);
            if (course != null)
            {
                return _mapper.Map<CourseDto>(course);
            }

            return null;
        }
    }
}
