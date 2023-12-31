﻿using Microsoft.AspNetCore.Mvc;
using SqlUniversity.Model.Dtos;
using SqlUniversity.Model.Requests;
using SqlUniversity.Services;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace SqlUniversity.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CourseRulesController : UniversityControllerBase
    {
        private readonly ICourseService _courseService;

        public CourseRulesController(ICourseService courseService)
        {
            this._courseService = courseService;
        }
        // GET: api/<CourseRules>
        [HttpGet]
        public async Task<IEnumerable<CourseRuleDto>> Get()
        {
            return _courseService.GetAllCourseRules();
        }

        // POST api/<CourseRules>
        [HttpPost]
        public async Task<CourseRuleResponse> Post([FromBody] CourseRuleRequest request)
        {
            return await ErrorWrapper(request,async (req) => _courseService.AddCourseRule(req));
        }
    }
}
