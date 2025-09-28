using Clinic.API.API.Dtos.AuthDtos;
using FluentValidation;
namespace Clinic.API.API.Validators.AuthValidators
{

    public class SmsConfirmDtoValidator : AbstractValidator<SmsConfirmDto>
    {
        public SmsConfirmDtoValidator()
        {
            RuleFor(x => x.PhoneNumber)
                .Matches(@"^(010|011|012|015)\d{8}$")
                .WithMessage("Phone number must be 11 digits and start with 010, 011, 012, or 015")
                .When(x => !string.IsNullOrWhiteSpace(x.PhoneNumber));

            // Code validation
            RuleFor(x => x.Code)
                .Cascade(CascadeMode.Stop)
                .NotEmpty().WithMessage("Reset code is required")
                .Length(6).WithMessage("Reset code must be 6 digits")
                .Matches("^[0-9]{6}$").WithMessage("Reset code must contain only digits");
        }
    }

}
