using Microsoft.AspNetCore.Mvc;
using SqlUniversity.Model.Dtos;
using SqlUniversity.Model.Requests;
using SqlUniversity.Services;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace SqlUniversity.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EnrollmentController : UniversityControllerBase
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
            return await ErrorWrapper<CreateEnrollmentRequest, CreateEnrollmentResponse>(request,async (req) => await _enrollmentService.CreateRegistrationAsync(req));
        }

        [HttpPut("{enrollmentId}")]
        public async Task<AddCoursesEnrollmentResponse> Put(int enrollmentId, [FromBody] AddCoursesEnrollmentRequest request)
        {
            return await ErrorWrapper(request,async (req) => await _enrollmentService.AddCourseToEnrollmentAsync(enrollmentId, req));
        }


        [HttpPut()]
        [Route($"{RemoveCoursesRoute}/{{enrollmentId}}")]
        public async Task<RemoveCoursesEnrollmentResponse> RemoveCourses(int enrollmentId, [FromBody] RemoveCoursesEnrollmentRequest req)
        {
            return await ErrorWrapper(req,async (req) => await _enrollmentService.RemoveCoursesAsync(enrollmentId, req));
        }


        [HttpPut()]
        [Route("removeallcourses/{enrollmentId}")]
        public async Task<RemoveAllCoursesEnrollmentResponse> RemoveAllCourses(int enrollmentId, [FromBody] RemoveAllCoursesEnrollmentRequest request)
        {
            return await ErrorWrapper(request,async (req) => await _enrollmentService.RemoveAllCoursesAsync(enrollmentId, request));
        }

        [HttpPut()]
        [Route($"{FinishRegistrationRoute}/{{enrollmentId}}")]
        public async Task<FinishRegistrationEnrollmentResponse> FinishRegistration(int enrollmentId, [FromBody] FinishRegistrationEnrollmentRequest request)
        {
            return await ErrorWrapper(request,async (req) =>  await _enrollmentService.FinishRegistrationAsync(enrollmentId, req));
        }


        [HttpPut()]
        [Route($"{PaidRoute}/{{enrollmentId}}")]
        public async Task<PaidEnrollmentResponse> Paid(int enrollmentId, [FromBody] PaidEnrollmentRequest request)
        {
            return await ErrorWrapper(request,async (req) => await _enrollmentService.PayedRegistrationAsync(enrollmentId, req));
        }

        [HttpDelete()]
        [Route($"{CancelRoute}/{{enrollmentId}}")]
        public async Task<CancelledEnrollmentResponse> Cancelled(int enrollmentId)
        {
            return await ErrorWrapper<CancelledEnrollmentRequest, CancelledEnrollmentResponse>(null,async (req) => await _enrollmentService.CancelledRegistrationAsync(enrollmentId));
        }

        // GET: api/<EnrollmentController>
        [HttpGet]
        public IEnumerable<EnrollmentDto> Get()
        {
            return _enrollmentService.GetAllEnrollments();
        }
    }
}
