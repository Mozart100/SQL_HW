using SqlUniversity.Model.Dtos;
using SqlUniversity.Model.Requests;
using System.Net.Http.Json;
using System.Text.Json;
using Xunit;

namespace SqlUniversity.Automation.Scenario
{
    public class StaticDataScenario : ScenarioBase
    {
        private readonly List<CourseRequest> _courseRequests;
        private readonly List<CourseRuleRequest> _courseRules;

        public StaticDataScenario(string baseUrl) : base(baseUrl)
        {
            CourseUrl = $"{baseUrl}/Courses";
            CourseRuleUrl = $"{baseUrl}/CourseRules";

            _courseRequests = new List<CourseRequest>
            {
                new  CourseRequest { Name ="Math" , LectureId = 1 ,IsMandatoryCourse = true , CoursePoints = 5, YearsSuited = new []{1,2 } },
                new  CourseRequest { Name ="English" , LectureId = 5,IsMandatoryCourse = true , CoursePoints = 5, YearsSuited = new []{1,2 } },
                new  CourseRequest { Name ="Geometry" , LectureId = 3,IsMandatoryCourse = true , CoursePoints = 5, YearsSuited = new []{1,2 } },



                new  CourseRequest { Name ="Drawing" , LectureId = 4,IsMandatoryCourse = false , CoursePoints = 2, YearsSuited = new []{1,2 } },
                new  CourseRequest { Name ="Russian" , LectureId = 4, IsMandatoryCourse = false , CoursePoints = 2, YearsSuited = new []{1,2 } },
                new  CourseRequest { Name ="Italian" , LectureId = 4, IsMandatoryCourse = false , CoursePoints = 2, YearsSuited = new []{1,2 } },
                new  CourseRequest { Name ="Arabic" , LectureId = 4, IsMandatoryCourse = false , CoursePoints = 2, YearsSuited = new []{1,2 } },


                new  CourseRequest { Name ="Nature" , LectureId = 4, IsMandatoryCourse = false , CoursePoints = 1, YearsSuited = new []{1,2 } },
                new  CourseRequest { Name ="Cinima" , LectureId = 4, IsMandatoryCourse = false , CoursePoints = 1, YearsSuited = new []{1,2 } },

            };

            _courseRules = new List<CourseRuleRequest> {
                new CourseRuleRequest { RequiredPoints= 18, Year=1 },
                new CourseRuleRequest { RequiredPoints= 8, Year=2 },
                new CourseRuleRequest { RequiredPoints= 8, Year=3 },
                new CourseRuleRequest { RequiredPoints= 8, Year=4 },
            };
        }

        public string CourseRuleUrl { get; }
        public string CourseUrl { get; }

        public override string ScenarioName => "Courses Scenario";

        protected override async Task RunScenario()
        {
            await PopulateCourseRules();

            await PopulateCourses();

        }

        private async Task PopulateCourseRules()
        {
            foreach (var courseRule in _courseRules)
            {
                var response = await RunPostCommand<CourseRuleRequest, CourseRuleResponse>(CourseRuleUrl, courseRule);
            }

            var courses = await Get<IEnumerable<CourseRuleDto>>(CourseRuleUrl);
            Assert.Equal(_courseRules.Count, courses.Count());
        }

        private async Task PopulateCourses()
        {
            foreach (var courseRequest in _courseRequests)
            {
                await RunPostCommand<CourseRequest, CourseDto>(CourseUrl, courseRequest);
            }

            var courses = await Get<IEnumerable<CourseDto>>(CourseUrl);
            Assert.Equal(_courseRequests.Count, courses.Count());
        }
    }
}
