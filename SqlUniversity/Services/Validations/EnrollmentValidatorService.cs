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
        IEnumerable<UniversityError> ValidateCreateEnrollmentRequest(CreateEnrollmentRequest request);
        IEnumerable<UniversityError> ValidationErrorsAddCourseToEnrollment(int enrollmentId, AddCoursesEnrollmentRequest request, out Enrollment enrollment);
        IEnumerable<UniversityError> RemoveCoursesEnrollmentRequest(int enrollmentId, RemoveCoursesEnrollmentRequest request, out Enrollment enrollment);
        IEnumerable<UniversityError> RemoveAllCoursesEnrollmentRequest(int enrollmentId, RemoveAllCoursesEnrollmentRequest request, out Enrollment enrollment);
        IEnumerable<UniversityError> FinishRegistrationEnrollmentRequest(int enrollmentId, FinishRegistrationEnrollmentRequest request, out Enrollment enrollment);
        IEnumerable<UniversityError> FinishRegistrationEnrollmentRequest(int enrollmentId, PaidEnrollmentRequest request, out Enrollment enrollment);
        IEnumerable<UniversityError> CancellationRegistrationEnrollmentRequest(int enrollmentId, out Enrollment enrollment);
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

        public IEnumerable<UniversityError> ValidateCreateEnrollmentRequest(CreateEnrollmentRequest request)
        {
            var validator = new CreateEnrollmentRequestValidator();
            var validationResult = validator.Validate(request);

            var errors = Dissect(validationResult);
            if (errors.Any())
            {
                return errors;
            }


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

            return errors;
        }

        public IEnumerable<UniversityError> ValidationErrorsAddCourseToEnrollment(int enrollmentId,
           AddCoursesEnrollmentRequest request,
           out Enrollment enrollment)
        {
            var validationEnrollmentErrors = ValidateEnrollment(enrollmentId, out enrollment);
            if (validationEnrollmentErrors.Any())
            {
                return validationEnrollmentErrors;
            }

            var validator = new AddCoursesEnrollmentRequestValidator();
            var validationResult = validator.Validate(request);
            var validationResultErrors = Dissect(validationResult);
            if (validationResultErrors.Any())
            {
                return validationResultErrors;
            }

            var errors = new List<UniversityError>();
            foreach (var courseId in request.CoursesIds)
            {
                if (!_courseIds.Contains(courseId))
                {
                    errors.Add(new UniversityError(errorMessage: $"This {courseId} doesnt exist", propertyName: nameof(courseId)));
                }
            }

            //Courses can be added from any states except from payed and cancelled.
            if (enrollment.TypeState == EnrollmentTypeState.Payed || enrollment.TypeState == EnrollmentTypeState.Cancelled)
            {
                errors.Add(new UniversityError(errorMessage: $"In {enrollment.TypeState} it is invalid to add course.", propertyName: "State"));
            }

            if (errors.Any())
            {
                return errors;
            }

            return errors;
        }



        public IEnumerable<UniversityError> RemoveCoursesEnrollmentRequest(int enrollmentId,
           RemoveCoursesEnrollmentRequest request,
          out Enrollment enrollment)
        {
            var validationEnrollmentErrors = ValidateEnrollment(enrollmentId, out enrollment);
            if (validationEnrollmentErrors.Any())
            {
                return validationEnrollmentErrors;
            }

            var validator = new RemoveCoursesEnrollmentRequestValidator();
            var validationResult = validator.Validate(request);
            var validationResultErrors = Dissect(validationResult);
            if (validationResultErrors.Any())
            {
                return validationResultErrors;
            }

            var errors = new List<UniversityError>();
            foreach (var courseId in request.CoursesIds)
            {
                if (!_courseIds.Contains(courseId))
                {
                    errors.Add(new UniversityError(errorMessage: $"This {courseId} doesnt exist", propertyName: nameof(courseId)));
                }
            }

            //Courses can be removed from any states except from payed and cancelled.
            if (enrollment.TypeState == EnrollmentTypeState.Payed || enrollment.TypeState == EnrollmentTypeState.Cancelled)
            {
                errors.Add(new UniversityError(errorMessage: $"In {enrollment.TypeState} it is invalid to remove course.", propertyName: "State"));
            }

            if (errors.Any())
            {
                return errors;
            }


            var isEnrollmentInactive = EnrollmentService.IsEnrollmentInactive(enrollment.TypeState);
            if (isEnrollmentInactive)
            {
                errors.Add(new UniversityError("This enrollment is no longer active", "EnrollmentState"));
                return errors;
            }

            return errors;
        }


        public IEnumerable<UniversityError> RemoveAllCoursesEnrollmentRequest(int enrollmentId,
                RemoveAllCoursesEnrollmentRequest request,
                out Enrollment enrollment)
        {
            var validationEnrollmentErrors = ValidateEnrollment(enrollmentId, out enrollment);
            if (validationEnrollmentErrors.Any())
            {
                return validationEnrollmentErrors;
            }
            
            var errors = new List<UniversityError>();


            if (enrollment.TypeState == EnrollmentTypeState.Payed || enrollment.TypeState == EnrollmentTypeState.Cancelled)
            {
                errors.Add(new UniversityError(errorMessage: $"In {enrollment.TypeState} it is invalid to remove all course.", propertyName: "State"));
                return errors;
            }

            var isEnrollmentInactive = EnrollmentService.IsEnrollmentInactive(enrollment.TypeState);
            if (isEnrollmentInactive)
            {
                errors.Add(new UniversityError("This enrollment is no longer active", "EnrollmentState"));
                return errors;
            }

            return errors;
        }

        public IEnumerable<UniversityError> FinishRegistrationEnrollmentRequest(int enrollmentId,
            FinishRegistrationEnrollmentRequest request,
            out Enrollment enrollment)
        {
            var validationErrors = ValidateEnrollment(enrollmentId, out enrollment);

            if (validationErrors.Any())
            {
                return validationErrors;
            }

            //Its stating in order to finish registration at least one course should be selected.
            var errors = new List<UniversityError>();
            if (enrollment.Courses.IsNullOrEmpty())
            {
                errors.Add(new UniversityError("No courses was selected", "Courses"));
            }

            if (enrollment.TypeState != EnrollmentTypeState.InProgress)
            {
                errors.Add(new UniversityError(errorMessage: $"Only form In InProgress state an enrollment can be finishedh.", propertyName: "State"));
                return errors;
            }

            return errors;
        }

        public IEnumerable<UniversityError> FinishRegistrationEnrollmentRequest(int enrollmentId,
            PaidEnrollmentRequest request,
            out Enrollment enrollment)
        {
            var validationErrors = ValidateEnrollment(enrollmentId, out enrollment);

            if (validationErrors.Any())
            {
                return validationErrors;
            }

            //Payment can be only in state completed.
            var errors = new List<UniversityError>();
            if (enrollment.TypeState != EnrollmentTypeState.Completed)
            {
                errors.Add(new UniversityError("Payment can be only in state completed", "State"));
            }

            return errors;
        }

        public IEnumerable<UniversityError> CancellationRegistrationEnrollmentRequest(int enrollmentId,
            out Enrollment enrollment)
        {
            var validationErrors = ValidateEnrollment(enrollmentId, out enrollment);

            if (validationErrors.Any())
            {
                return validationErrors;
            }

            //Cancellation can be only from Completed or InProgress
            var errors = new List<UniversityError>();
            if (enrollment.TypeState != EnrollmentTypeState.Completed && enrollment.TypeState != EnrollmentTypeState.InProgress)
            {
                errors.Add(new UniversityError("Cancellation can be only from completed or from InProgress", "State"));
            }

            return errors;
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
