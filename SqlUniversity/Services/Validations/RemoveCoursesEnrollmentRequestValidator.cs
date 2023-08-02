using FluentValidation;
using SqlUniversity.Model.Requests;

namespace SqlUniversity.Services.Validations
{
    public class RemoveCoursesEnrollmentRequestValidator : AbstractValidator<RemoveCoursesEnrollmentRequest>
    {
        public RemoveCoursesEnrollmentRequestValidator()
        {
            RuleFor(request => request.CoursesIds).NotEmpty().WithMessage("In order to remove a course it should be added!");
        }
    }
}
