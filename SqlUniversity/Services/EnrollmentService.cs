using AutoMapper;
using SqlUniversity.DataAccess.Models;
using SqlUniversity.DataAccess.Repository;
using SqlUniversity.Infrastracture;
using SqlUniversity.Model.Dtos;
using SqlUniversity.Model.Requests;
using SqlUniversity.Services.EnrollmentStateMachine;
using SqlUniversity.Services.Validations;
using System.Collections.Concurrent;

namespace SqlUniversity.Services
{
    public interface IEnrollmentService
    {
        CreateEnrollmentResponse CreateRegistration(CreateEnrollmentRequest enrollment);
        AddCoursesEnrollmentResponse AddCourseToEnrollment(int enrollmentId, AddCoursesEnrollmentRequest request);

        FinishRegistrationEnrollmentResponse FinishRegistration(int enrollmentId, FinishRegistrationEnrollmentRequest request);
        IEnumerable<EnrollmentDto> GetAllEnrollments();
        PaidEnrollmentResponse PayedRegistration(int enrollmentId, PaidEnrollmentRequest request);
        RemoveAllCoursesEnrollmentResponse RemoveAllCourses(int enrollmentId, RemoveAllCoursesEnrollmentRequest request);
        RemoveCoursesEnrollmentResponse RemoveCourses(int enrollmentId, RemoveCoursesEnrollmentRequest request);
        CancelledEnrollmentResponse CancelledRegistration(int enrollmentId);
    }

    public class EnrollmentService : IEnrollmentService
    {
        private readonly ILogger<EnrollmentService> _logger;
        private readonly IMapper _mapper;
        private readonly IEnrollmentRepository _enrollmentRepository;
        private readonly ICourseService _courseService;
        private readonly IAcademicProcessorService _academicProcessorService;
        private readonly IEnrollmentValidatorService _enrollmentValidatorService;
        private readonly ConcurrentDictionary<int, EnrollmentStateMachine.EnrollmentStateMachine> _enrollments;

        public EnrollmentService(ILogger<EnrollmentService> logger,
            IMapper mapper,
            IEnrollmentRepository enrollmentRepository,
            ICourseService courseService,
            IAcademicProcessorService academicProcessorService,
            IEnrollmentValidatorService enrollmentValidatorService
            )
        {
            this._logger = logger;
            this._mapper = mapper;
            this._enrollmentRepository = enrollmentRepository;
            this._courseService = courseService;
            this._academicProcessorService = academicProcessorService;
            this._enrollmentValidatorService = enrollmentValidatorService;

            _enrollments = new ConcurrentDictionary<int, EnrollmentStateMachine.EnrollmentStateMachine>();

        }

        public CreateEnrollmentResponse CreateRegistration(CreateEnrollmentRequest request)
        {
            var errors = _enrollmentValidatorService.ValidateCreateEnrollmentRequest(request);
            if (errors.IsNullOrEmpty() == false)
            {
                var errorResponse = new CreateEnrollmentResponse
                {
                    Message = "During registration an error occurred!",
                    IsOperationPassed = false,
                    Errors = errors
                };

                return errorResponse;
            }

            var enrollmentDto = new EnrollmentDto
            {
                TypeState = EnrollmentTypeState.InProgress,
                EnrollmentForYear = 1, //First year
                StuentId = request.StuentId,
            };

            var enrollmentState = new EnrollmentInProgressState(_academicProcessorService.GetAcademicYearDataByYear(enrollmentDto.EnrollmentForYear),
                _courseService.GetAllCourse(),
                _enrollmentRepository, _mapper);
            var stateController = new EnrollmentStateMachine.EnrollmentStateMachine(enrollmentState);

            var saveEnrollment = enrollmentState.SaveState(enrollmentDto);


            _enrollments.GetOrAdd(saveEnrollment.Id, stateController);

            _logger.LogInformation("enrollmentRequest started {EnrollmentRequest}", saveEnrollment.Id);
            var response = _mapper.Map<CreateEnrollmentResponse>(saveEnrollment);
            response.Message = "Registration just started please enroll to courses.";
            response.IsOperationPassed = true;
            return response;
        }


     

        public AddCoursesEnrollmentResponse AddCourseToEnrollment(int enrollmentId, AddCoursesEnrollmentRequest request)
        {
            var errors = _enrollmentValidatorService.ValidationErrorsAddCourseToEnrollment(enrollmentId, request, out var enrollment);
            if (!errors.IsNullOrEmpty())
            {
                var errorResponse = _mapper.Map<AddCoursesEnrollmentResponse>(enrollment ?? new Enrollment());
                errorResponse.Message = $"During adding new courses an error occured!!!";
                errorResponse.IsOperationPassed = false;
                errorResponse.Errors = errors;
                return errorResponse;
            }

            if (_enrollments.TryGetValue(enrollmentId, out var controller))
            {
                if (controller.TryCaseToState(out IChangeCoursesState state))
                {

                    state.AddCourses(request.CoursesIds);
                    var response = _mapper.Map<AddCoursesEnrollmentResponse>(state.EnrollmentDto);
                    response.IsOperationPassed = true;
                    return response;
                }
            }

            throw new Exception($"{nameof(AddCourseToEnrollment)} - Failed!");
        }

        public RemoveCoursesEnrollmentResponse RemoveCourses(int enrollmentId, RemoveCoursesEnrollmentRequest request)
        {
            var errors = _enrollmentValidatorService.RemoveCoursesEnrollmentRequest(enrollmentId, request, out var enrollment);
            if (!errors.IsNullOrEmpty())
            {
                var errorResponse = _mapper.Map<RemoveCoursesEnrollmentResponse>(enrollment ?? new Enrollment());
                errorResponse.Message = $"During removing courses an error occured!!!";
                errorResponse.IsOperationPassed = false;
                errorResponse.Errors = errors;
                return errorResponse;

            }


            if (_enrollments.TryGetValue(enrollmentId, out var controller))
            {
                if (controller.TryCaseToState(out IChangeCoursesState state))
                {

                    state.RemoveCourses(request.CoursesIds);
                    var response = _mapper.Map<RemoveCoursesEnrollmentResponse>(state.EnrollmentDto);
                    response.IsOperationPassed = true;
                    return response;
                }
            }

            throw new Exception($"{nameof(RemoveCourses)} - Failed!");

        }

        public RemoveAllCoursesEnrollmentResponse RemoveAllCourses(int enrollmentId, RemoveAllCoursesEnrollmentRequest request)
        {
            var errors = _enrollmentValidatorService.RemoveAllCoursesEnrollmentRequest(enrollmentId, request, out var enrollment);
            if (!errors.IsNullOrEmpty())
            {
                var errorResponse = _mapper.Map<RemoveAllCoursesEnrollmentResponse>(enrollment ?? new Enrollment());
                errorResponse.Message = $"During removing all courses an error occured!!!";
                errorResponse.IsOperationPassed = false;
                errorResponse.Errors = errors;
                return errorResponse;

            }

            if (_enrollments.TryGetValue(enrollmentId, out var controller))
            {
                if (controller.TryCaseToState(out IChangeCoursesState state))
                {
                    var coursesIds = state.EnrollmentDto.Courses;
                    state.RemoveCourses(coursesIds);
                    var response = _mapper.Map<RemoveAllCoursesEnrollmentResponse>(state.EnrollmentDto);
                    response.IsOperationPassed = true;
                    return response;
                }
            }

            throw new Exception($"{nameof(RemoveAllCourses)} - Failed!");
        }

        public FinishRegistrationEnrollmentResponse FinishRegistration(int enrollmentId, FinishRegistrationEnrollmentRequest request)
        {
            var errors = _enrollmentValidatorService.FinishRegistrationEnrollmentRequest(enrollmentId, request, out var enrollment);
            if (!errors.IsNullOrEmpty())
            {
                var errorResponse = _mapper.Map<FinishRegistrationEnrollmentResponse>(enrollment ?? new Enrollment());
                errorResponse.Message = $"During Finishing registration an error occured!!!";
                errorResponse.IsOperationPassed = false;
                errorResponse.Errors = errors;
                return errorResponse;
            }


            if (_enrollments.TryGetValue(enrollmentId, out var controller))
            {
                if (controller.TryCaseToState(out ICompleteRegistrationState state))
                {
                    //var coursesIds = state.EnrollmentDto.Courses;
                    state.ChangedToCompletion();
                    var response = _mapper.Map<FinishRegistrationEnrollmentResponse>(state.EnrollmentDto);
                    response.IsOperationPassed = true;
                    return response;
                }
            }

            throw new Exception($"{nameof(FinishRegistration)} - Failed!");
        }

        public PaidEnrollmentResponse PayedRegistration(int enrollmentId, PaidEnrollmentRequest request)
        {
            var errors = _enrollmentValidatorService.FinishRegistrationEnrollmentRequest(enrollmentId, request, out var enrollment);
            if (!errors.IsNullOrEmpty())
            {
                var errorResponse = _mapper.Map<PaidEnrollmentResponse>(enrollment);
                errorResponse.Message = $"During payment an error occured!!!";
                errorResponse.IsOperationPassed = false;
                errorResponse.Errors = errors;
                return errorResponse;
            }

            if (_enrollments.TryGetValue(enrollmentId, out var controller))
            {
                if (controller.TryCaseToState(out IPaidState state))
                {
                    state.ChangedToPaidState();
                    var response = _mapper.Map<PaidEnrollmentResponse>(state.EnrollmentDto);
                    response.IsOperationPassed = true;
                    return response;
                }
            }

            throw new Exception($"{nameof(PayedRegistration)} - Failed!");
        }

        public CancelledEnrollmentResponse CancelledRegistration(int enrollmentId)
        {
            var errors = _enrollmentValidatorService.CancellationRegistrationEnrollmentRequest(enrollmentId, out var enrollment);
            if (!errors.IsNullOrEmpty())
            {
                var errorResponse = _mapper.Map<CancelledEnrollmentResponse>(enrollment ?? new Enrollment());
                errorResponse.Message = $"During cancellation an error occured!!!";
                errorResponse.IsOperationPassed = false;
                errorResponse.Errors = errors;
                return errorResponse;
            }

            if (_enrollments.TryGetValue(enrollmentId, out var controller))
            {
                if (controller.TryCaseToState(out ICancelledState state))
                {
                    state.ChangedToCancelState();
                    var response = _mapper.Map<CancelledEnrollmentResponse>(state.EnrollmentDto);
                    response.IsOperationPassed = true;
                    return response;
                }
            }

            throw new Exception($"{nameof(CancelledRegistration)} - Failed!");

        }

        public IEnumerable<EnrollmentDto> GetAllEnrollments()
        {
            var enrollments = new List<EnrollmentDto>();

            foreach (var enrollment in _enrollmentRepository.GetAll())
            {
                enrollments.Add(_mapper.Map<EnrollmentDto>(enrollment));
            }

            return enrollments;
        }

        #region States

        public static bool IsEnrollmentFinished(EnrollmentTypeState state)
        {
            return state == EnrollmentTypeState.Cancelled || state == EnrollmentTypeState.Payed;
        }

        public static bool IsEnrollmentInactive(EnrollmentTypeState state)
        {
            return state == EnrollmentTypeState.Cancelled || state == EnrollmentTypeState.Payed;
        }

        #endregion
    }
}
