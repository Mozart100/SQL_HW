using Microsoft.AspNetCore.Mvc;
using SqlUniversity.Model.Dtos;
using SqlUniversity.Model.Requests;
using SqlUniversity.Services;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace SqlUniversity.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CoursesController : UniversityControllerBase
    {
        private readonly ICourseService _courseService;

        public CoursesController(ICourseService courseService)
        {
            this._courseService = courseService;
        }
        // GET: api/<CoursesController>
        [HttpGet]
        public async Task<IEnumerable<CourseDto>> Get()
        {
            return _courseService.GetAllCourse();
        }

        // POST api/<CoursesController>
        [HttpPost]
        public async Task<CourseResponse> Post([FromBody] CourseRequest value)
        {
            return await ErrorWrapper<CourseRequest, CourseResponse>(async () => _courseService.AddCourse(value));
        }
    }
}
