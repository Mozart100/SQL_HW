using Microsoft.AspNetCore.Mvc;
using SqlUniversity.Model.Dtos;
using SqlUniversity.Model.Requests;
using SqlUniversity.Services;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace SqlUniversity.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EnrollmentController : ControllerBase
    {
        public const string RegistrationRoute = "registration";
        public const string RemoveCoursesRoute = "removecourses";
        public const string FinishRegistrationRoute = "finishregistration";
        public const string CancelRoute = "cancelled";
        public const string PaidRoute = "paid";

        private readonly IEnrollmentService _enrollmentService;

        public EnrollmentController(IEnrollmentService enrollmentService)
        {
            this._enrollmentService = enrollmentService;
        }


        [HttpPost]
        [Route(RegistrationRoute)]
        public async Task<CreateEnrollmentResponse> Registration([FromBody] CreateEnrollmentRequest request)
        {
            var response = _enrollmentService.CreateRegistration(request);
            return response;
        }

        [HttpPut("{enrollmentId}")]
        public async Task<AddCoursesEnrollmentResponse> Put(int enrollmentId, [FromBody] AddCoursesEnrollmentRequest request)
        {
            var dto = _enrollmentService.AddCourseToEnrollment(enrollmentId, request);
            return dto;
        }


        [HttpPut()]
        [Route($"{RemoveCoursesRoute}/{{enrollmentId}}")]
        public async Task<RemoveCoursesEnrollmentResponse> RemoveCourses(int enrollmentId, [FromBody] RemoveCoursesEnrollmentRequest request)
        {
            var dto = _enrollmentService.RemoveCourses(enrollmentId, request);
            return dto;
        }


        [HttpPut()]
        [Route("removeallcourses/{enrollmentId}")]
        public async Task<RemoveAllCoursesEnrollmentResponse> RemoveAllCourses(int enrollmentId, [FromBody] RemoveAllCoursesEnrollmentRequest request)
        {
            var dto = _enrollmentService.RemoveAllCourses(enrollmentId, request);
            return dto;
        }

        [HttpPut()]
        [Route($"{FinishRegistrationRoute}/{{enrollmentId}}")]
        public async Task<FinishRegistrationEnrollmentResponse> FinishRegistration(int enrollmentId, [FromBody] FinishRegistrationEnrollmentRequest request)
        {
            FinishRegistrationEnrollmentResponse dto = _enrollmentService.FinishRegistration(enrollmentId, request);
            return dto;
        }


        [HttpPut()]
        [Route($"{PaidRoute}/{{enrollmentId}}")]
        public async Task<PaidEnrollmentResponse> Paid(int enrollmentId, [FromBody] PaidEnrollmentRequest request)
        {
            var dto = _enrollmentService.PayedRegistration(enrollmentId, request);
            return dto;
        }

        [HttpDelete()]
        [Route($"{CancelRoute}/{{enrollmentId}}")]
        public async Task<CancelledEnrollmentResponse> Cancelled(int enrollmentId)
        {
            CancelledEnrollmentResponse dto = _enrollmentService.CancelledRegistration(enrollmentId);
            return dto;
        }

        // GET: api/<EnrollmentController>
        [HttpGet]
        public IEnumerable<EnrollmentDto> Get()
        {
            return _enrollmentService.GetAllEnrollments();
        }
    }
}
