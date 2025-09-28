using Clinic.API.API.Dtos.AuthDtos;
using FluentValidation;

namespace Clinic.API.API.Validators.AuthValidators
{
    public class EmailConfirmDtoValidator : AbstractValidator<EmailConfirmDto>
    {
        public EmailConfirmDtoValidator()
        {
            RuleFor(x => x.Email)
                .Cascade(CascadeMode.Stop)
                .NotEmpty().WithMessage("Email is required")
                .EmailAddress().WithMessage("Email Address must be valid");


            RuleFor(x => x.Code)
                .Cascade(CascadeMode.Stop)
                .NotEmpty().WithMessage("Confirmation code is required")
                .Length(6).WithMessage("Confirmation code must be 6 digits")
                .Matches("^[0-9]{6}$").WithMessage("Confirmation code must contain only digits");
        }
    }
}
