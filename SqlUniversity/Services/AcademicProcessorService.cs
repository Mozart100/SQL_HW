using AutoMapper;
using SqlUniversity.DataAccess.Repository;
using SqlUniversity.Model.Dtos;
using System.Collections.Concurrent;

namespace SqlUniversity.Services
{
    public interface IAcademicProcessorService
    {
        AcademicYearDto GetAcademicYearDataByYear(int enrollmentForYear);
    }

    public class AcademicProcessorService : IAcademicProcessorService
    {
        private readonly ILogger<AcademicProcessorService> _logger;
        private readonly IMapper _mapper;
        private readonly ICoursetRepository _coursetRepository;
        private readonly IAssignCourseYearyRepository _assignCourseYearyRepository;
        private readonly ICourseRulesRepository _courseRulesRepository;

        private readonly ConcurrentDictionary<int, AcademicYearDto> _academicYears;

        public AcademicProcessorService(ILogger<AcademicProcessorService> logger,
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

            _academicYears = new ConcurrentDictionary<int, AcademicYearDto>();
            Populate();
        }

        public AcademicYearDto GetAcademicYearDataByYear(int enrollmentForYear)
        {
            if(_academicYears.TryGetValue(enrollmentForYear,out var academicYearDto))
            {
                return academicYearDto;
            }

            return null;
        }

        private void Populate()
        {
            foreach (var course in _coursetRepository.GetAll())
            {
                var assignCourse = _assignCourseYearyRepository.Get(x => x.CourseId == course.Id);
                var courseRule = _courseRulesRepository.Get(x => x.Year == assignCourse.Year);

                if (assignCourse != null && courseRule != null)
                {
                    if (_academicYears.TryGetValue(assignCourse.Year, out var academicYear))
                    {
                        if(course.IsMandatoryCourse)
                        {
                            academicYear.MandatoryCourses.Add(course.Id);
                        }
                    }
                    else
                    {
                        _academicYears[assignCourse.Year] = new AcademicYearDto(assignCourse.Year, courseRule.RequiredPoints, Enumerable.Empty<int>());
                    }
                }
            }
        }
    }
}
