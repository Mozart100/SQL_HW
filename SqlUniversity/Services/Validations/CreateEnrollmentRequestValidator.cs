using FluentValidation;
using SqlUniversity.Model.Requests;

namespace SqlUniversity.Services.Validations
{
    public class CreateEnrollmentRequestValidator : AbstractValidator<CreateEnrollmentRequest>
    {
        public CreateEnrollmentRequestValidator()
        {
            RuleFor(request => request.StuentId).GreaterThan(0).WithMessage("{PropertyName} should be greater than 0");
        }
    }
}
