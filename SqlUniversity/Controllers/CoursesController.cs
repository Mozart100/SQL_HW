using Microsoft.AspNetCore.Mvc;
using SqlUniversity.Model.Dtos;
using SqlUniversity.Model.Requests;
using SqlUniversity.Services;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace SqlUniversity.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CoursesController : ControllerBase
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
        public async Task<ActionResult<int>> Post([FromBody] CourseRequest value)
        {
            var courseId = _courseService.AddCourse(value);
            return Ok(courseId);
        }
    }
}
