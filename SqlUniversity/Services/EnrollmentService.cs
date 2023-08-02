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
        Task<CreateEnrollmentResponse> CreateRegistrationAsync(CreateEnrollmentRequest enrollment);
        Task<AddCoursesEnrollmentResponse> AddCourseToEnrollmentAsync(int enrollmentId, AddCoursesEnrollmentRequest request);

        Task<FinishRegistrationEnrollmentResponse> FinishRegistrationAsync(int enrollmentId, FinishRegistrationEnrollmentRequest request);
        IEnumerable<EnrollmentDto> GetAllEnrollments();
        Task<PaidEnrollmentResponse> PayedRegistrationAsync(int enrollmentId, PaidEnrollmentRequest request);
        Task<RemoveAllCoursesEnrollmentResponse> RemoveAllCoursesAsync(int enrollmentId, RemoveAllCoursesEnrollmentRequest request);
        Task<RemoveCoursesEnrollmentResponse> RemoveCoursesAsync(int enrollmentId, RemoveCoursesEnrollmentRequest request);
        Task<CancelledEnrollmentResponse> CancelledRegistrationAsync(int enrollmentId);
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

        public async Task<CreateEnrollmentResponse> CreateRegistrationAsync(CreateEnrollmentRequest request)
        {
            await _enrollmentValidatorService.ValidateCreateEnrollmentRequestAsync(request);

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

            var response = _mapper.Map<CreateEnrollmentResponse>(saveEnrollment);
            response.IsOperationPassed = true;
            response.Request = request;

            return response;
        }




        public async Task<AddCoursesEnrollmentResponse> AddCourseToEnrollmentAsync(int enrollmentId, AddCoursesEnrollmentRequest request)
        {
            await _enrollmentValidatorService.ValidationErrorsAddCourseToEnrollmentAsync(enrollmentId, request);
            var enrollment = _enrollmentRepository.Get(x => x.Id == enrollmentId);

            if (_enrollments.TryGetValue(enrollmentId, out var controller))
            {
                if (controller.TryCaseToState(out IChangeCoursesState state))
                {

                    state.AddCourses(request.CoursesIds);
                    var response = _mapper.Map<AddCoursesEnrollmentResponse>(state.EnrollmentDto);
                    response.IsOperationPassed = true;
                    response.Request = request;
                    return response;
                }
            }

            throw new Exception($"{nameof(AddCoursesEnrollmentRequest)} - Failed!");
        }

        public async Task<RemoveCoursesEnrollmentResponse> RemoveCoursesAsync(int enrollmentId, RemoveCoursesEnrollmentRequest request)
        {
            await _enrollmentValidatorService.RemoveCoursesEnrollmentRequestAsync(enrollmentId, request);

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

            throw new Exception($"{nameof(RemoveCoursesAsync)} - Failed!");

        }

        public async Task<RemoveAllCoursesEnrollmentResponse> RemoveAllCoursesAsync(int enrollmentId, RemoveAllCoursesEnrollmentRequest request)
        {
            await _enrollmentValidatorService.RemoveAllCoursesEnrollmentRequestAsync(enrollmentId, request);

            if (_enrollments.TryGetValue(enrollmentId, out var controller))
            {
                if (controller.TryCaseToState(out IChangeCoursesState state))
                {
                    var coursesIds = state.EnrollmentDto.Courses;
                    state.RemoveCourses(coursesIds);
                    var response = _mapper.Map<RemoveAllCoursesEnrollmentResponse>(state.EnrollmentDto);
                    response.IsOperationPassed = true;
                    response.Request = request;
                    return response;
                }
            }

            throw new Exception($"{nameof(RemoveAllCoursesEnrollmentRequest)} - Failed!");
        }

        public async Task<FinishRegistrationEnrollmentResponse> FinishRegistrationAsync(int enrollmentId, FinishRegistrationEnrollmentRequest request)
        {
            await _enrollmentValidatorService.FinishRegistrationEnrollmentRequestAsync(enrollmentId, request);

            if (_enrollments.TryGetValue(enrollmentId, out var controller))
            {
                if (controller.TryCaseToState(out ICompleteRegistrationState state))
                {
                    //var coursesIds = state.EnrollmentDto.Courses;
                    state.ChangedToCompletion();
                    var response = _mapper.Map<FinishRegistrationEnrollmentResponse>(state.EnrollmentDto);
                    response.IsOperationPassed = true;
                    response.Request = request;
                    return response;
                }
            }

            throw new Exception($"{nameof(FinishRegistrationAsync)} - Failed!");
        }

        public async Task<PaidEnrollmentResponse> PayedRegistrationAsync(int enrollmentId, PaidEnrollmentRequest request)
        {
            await _enrollmentValidatorService.PaymentEnrollmentRequestAsync(enrollmentId, request);

            if (_enrollments.TryGetValue(enrollmentId, out var controller))
            {
                if (controller.TryCaseToState(out IPaidState state))
                {
                    state.ChangedToPaidState();
                    var response = _mapper.Map<PaidEnrollmentResponse>(state.EnrollmentDto);
                    response.IsOperationPassed = true;
                    response.Request = request;
                    return response;
                }
            }

            throw new Exception($"{nameof(PayedRegistrationAsync)} - Failed!");
        }

        public async Task<CancelledEnrollmentResponse> CancelledRegistrationAsync(int enrollmentId)
        {
            await _enrollmentValidatorService.CancellationRegistrationEnrollmentRequestAsync(enrollmentId);

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

            throw new Exception($"{nameof(CancelledRegistrationAsync)} - Failed!");

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
