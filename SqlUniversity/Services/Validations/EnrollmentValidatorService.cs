using SqlUniversity.DataAccess.Models;
using SqlUniversity.DataAccess.Repository;
using SqlUniversity.Infrastracture;
using SqlUniversity.Model.Requests;

namespace SqlUniversity.Services.Validations
{
    public class UniversityError
    {
        public UniversityError(string errorMessage, string propertyName)
        {
            PropertyName = propertyName;
            ErrorMessage = errorMessage;
        }

        public string ErrorMessage { get; }
        public string PropertyName { get; }

    }

    public interface IEnrollmentValidatorService
    {
        Task ValidateCreateEnrollmentRequestAsync(CreateEnrollmentRequest request);
        Task ValidationErrorsAddCourseToEnrollmentAsync(int enrollmentId, AddCoursesEnrollmentRequest request);
        Task RemoveCoursesEnrollmentRequestAsync(int enrollmentId, RemoveCoursesEnrollmentRequest request);
        Task RemoveAllCoursesEnrollmentRequestAsync(int enrollmentId, RemoveAllCoursesEnrollmentRequest request);
        Task FinishRegistrationEnrollmentRequestAsync(int enrollmentId, FinishRegistrationEnrollmentRequest request);
        Task PaymentEnrollmentRequestAsync(int enrollmentId, PaidEnrollmentRequest request);
        Task CancellationRegistrationEnrollmentRequestAsync(int enrollmentId);
    }

    public class EnrollmentValidatorService : ServiceValidatorBase, IEnrollmentValidatorService
    {
        private readonly ILogger<EnrollmentValidatorService> _logger;
        private readonly IEnrollmentRepository _enrollmentRepository;
        private readonly ICourseService _courseService;
        private readonly HashSet<int> _courseIds;

        public EnrollmentValidatorService(ILogger<EnrollmentValidatorService> logger,
            IEnrollmentRepository enrollmentRepository,
            ICourseService courseService
            )
        {
            this._logger = logger;
            this._enrollmentRepository = enrollmentRepository;
            this._courseService = courseService;

            _courseIds = new HashSet<int>(courseService.GetAllCourse().Select(x => x.Id));

        }

        public async Task ValidateCreateEnrollmentRequestAsync(CreateEnrollmentRequest request)
        {
            var validator = new CreateEnrollmentRequestValidator();
            var validationResult = validator.Validate(request);

            var errors = Dissect(validationResult);
            Validate(errors);


            var enrollments = _enrollmentRepository.GetAll();
            foreach (var enrollment in enrollments)
            {
                if (enrollment.StuentId == request.StuentId)
                {
                    if (EnrollmentService.IsEnrollmentFinished(enrollment.TypeState) == false)
                    {
                        errors.Add(new UniversityError(propertyName: "Student Id", errorMessage: "This user is under enrollemnt"));
                    }
                }
            }

            Validate(errors);
        }

        public async Task ValidationErrorsAddCourseToEnrollmentAsync(int enrollmentId, AddCoursesEnrollmentRequest request)
        {
            var validator = new AddCoursesEnrollmentRequestValidator();
            var validationResult = validator.Validate(request);
            var errors = Dissect(validationResult);
            Validate(errors);


            var enrollment = _enrollmentRepository.Get(x => x.Id == enrollmentId);
            if (enrollment == null)
            {
                errors.Add(new UniversityError(errorMessage: $"No souch enrollment {enrollmentId}", propertyName: nameof(enrollmentId)));
            }
            Validate(errors);

            foreach (var courseId in request.CoursesIds)
            {
                if (!_courseIds.Contains(courseId))
                {
                    errors.Add(new UniversityError(errorMessage: $"This {courseId} doesnt exist", propertyName: nameof(courseId)));
                }
            }
            Validate(errors);


            //Courses can be added from any states except from payed and cancelled.
            if (enrollment.TypeState == EnrollmentTypeState.Payed || enrollment.TypeState == EnrollmentTypeState.Cancelled)
            {
                errors.Add(new UniversityError(errorMessage: $"In {enrollment.TypeState} it is invalid to add course.", propertyName: "State"));
            }

            Validate(errors);
        }

        public async Task RemoveCoursesEnrollmentRequestAsync(int enrollmentId,
           RemoveCoursesEnrollmentRequest request)
        {
            var validator = new RemoveCoursesEnrollmentRequestValidator();
            var validationResult = validator.Validate(request);
            var errors = Dissect(validationResult);
            Validate(errors);



            var enrollment = _enrollmentRepository.Get(x => x.Id == enrollmentId);
            if (enrollment == null)
            {
                errors.Add(new UniversityError(errorMessage: $"No souch enrollment {enrollmentId}", propertyName: nameof(enrollmentId)));
            }
            Validate(errors);



            foreach (var courseId in request.CoursesIds)
            {
                if (!_courseIds.Contains(courseId))
                {
                    errors.Add(new UniversityError(errorMessage: $"This {courseId} doesnt exist", propertyName: nameof(courseId)));
                }
            }
            Validate(errors);


            //Courses can be removed from any states except from payed and cancelled.
            if (enrollment.TypeState == EnrollmentTypeState.Payed || enrollment.TypeState == EnrollmentTypeState.Cancelled)
            {
                errors.Add(new UniversityError(errorMessage: $"In {enrollment.TypeState} it is invalid to remove course.", propertyName: "State"));
            }
            Validate(errors);



            var isEnrollmentInactive = EnrollmentService.IsEnrollmentInactive(enrollment.TypeState);
            if (isEnrollmentInactive)
            {
                errors.Add(new UniversityError("This enrollment is no longer active", "EnrollmentState"));
            }
            Validate(errors);

        }


        public async Task RemoveAllCoursesEnrollmentRequestAsync(int enrollmentId,
                RemoveAllCoursesEnrollmentRequest request)
        {
            var errors = new List<UniversityError>();
            var enrollment = _enrollmentRepository.Get(x => x.Id == enrollmentId);
            if (enrollment == null)
            {
                errors.Add(new UniversityError(errorMessage: $"No souch enrollment {enrollmentId}", propertyName: nameof(enrollmentId)));
            }
            Validate(errors);



            if (enrollment.TypeState == EnrollmentTypeState.Payed || enrollment.TypeState == EnrollmentTypeState.Cancelled)
            {
                errors.Add(new UniversityError(errorMessage: $"In {enrollment.TypeState} it is invalid to remove all course.", propertyName: "State"));
            }
            Validate(errors);


            var isEnrollmentInactive = EnrollmentService.IsEnrollmentInactive(enrollment.TypeState);
            if (isEnrollmentInactive)
            {
                errors.Add(new UniversityError("This enrollment is no longer active", "EnrollmentState"));
            }
            Validate(errors);

        }

        public async Task FinishRegistrationEnrollmentRequestAsync(int enrollmentId,
            FinishRegistrationEnrollmentRequest request)
        {
            var errors = new List<UniversityError>();
            var enrollment = _enrollmentRepository.Get(x => x.Id == enrollmentId);
            if (enrollment == null)
            {
                errors.Add(new UniversityError(errorMessage: $"No souch enrollment {enrollmentId}", propertyName: nameof(enrollmentId)));
            }
            Validate(errors);

            //Its stating in order to finish registration at least one course should be selected.
            if (enrollment.Courses.IsNullOrEmpty())
            {
                errors.Add(new UniversityError("No courses was selected", "Courses"));
            }
            Validate(errors);


            if (enrollment.TypeState != EnrollmentTypeState.InProgress)
            {
                errors.Add(new UniversityError(errorMessage: $"Only form In InProgress state an enrollment can be finishedh.", propertyName: "State"));
            }
            Validate(errors);
        }

        public async Task PaymentEnrollmentRequestAsync(int enrollmentId,
            PaidEnrollmentRequest request)
        {
            var errors = new List<UniversityError>();
            var enrollment = _enrollmentRepository.Get(x => x.Id == enrollmentId);
            if (enrollment == null)
            {
                errors.Add(new UniversityError(errorMessage: $"No souch enrollment {enrollmentId}", propertyName: nameof(enrollmentId)));
            }
            Validate(errors);

            //Payment can be only in state completed.
            if (enrollment.TypeState != EnrollmentTypeState.Completed)
            {
                errors.Add(new UniversityError("Payment can be only in state completed", "State"));
            }

            Validate(errors);
        }

        public async Task CancellationRegistrationEnrollmentRequestAsync(int enrollmentId)
        {
            var errors = new List<UniversityError>();
            var enrollment = _enrollmentRepository.Get(x => x.Id == enrollmentId);
            if (enrollment == null)
            {
                errors.Add(new UniversityError(errorMessage: $"No souch enrollment {enrollmentId}", propertyName: nameof(enrollmentId)));
            }
            Validate(errors);

            //Cancellation can be only from Completed or InProgress
            if (enrollment.TypeState != EnrollmentTypeState.Completed && enrollment.TypeState != EnrollmentTypeState.InProgress)
            {
                errors.Add(new UniversityError("Cancellation can be only from completed or from InProgress", "State"));
            }
            Validate(errors);

        }

        private IEnumerable<UniversityError> ValidateEnrollment(int enrollmentId, out Enrollment enrollment)
        {
            var errors = new List<UniversityError>();
            enrollment = _enrollmentRepository.Get(x => x.Id == enrollmentId);
            if (enrollment == null)
            {
                errors.Add(new UniversityError(errorMessage: $"No souch enrollment {enrollmentId}", propertyName: nameof(enrollmentId)));
            }

            return errors;
        }
    }
}
