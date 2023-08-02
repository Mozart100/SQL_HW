using FluentValidation;
using SqlUniversity.Model.Requests;

namespace SqlUniversity.Services.Validations
{
    public class AddCoursesEnrollmentRequestValidator : AbstractValidator<AddCoursesEnrollmentRequest>
    {
        public AddCoursesEnrollmentRequestValidator()
        {
            RuleFor(request=> request.CoursesIds).NotEmpty().WithMessage("At least a single course should be added!");
        }
    }
}
