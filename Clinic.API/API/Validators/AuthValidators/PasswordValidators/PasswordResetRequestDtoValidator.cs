using Clinic.API.API.Dtos.AuthDtos.PasswordDtos;
using FluentValidation;
namespace Clinic.API.API.Validators.AuthValidators.PasswordValidators
{

    public class PasswordResetRequestDtoValidator : AbstractValidator<PasswordResetRequestDto>
    {
        public PasswordResetRequestDtoValidator()
        {
            // Either Email or PhoneNumber is required
            RuleFor(x => x)
                .Must(x => !string.IsNullOrWhiteSpace(x.Email) || !string.IsNullOrWhiteSpace(x.PhoneNumber))
                .WithMessage("Either Email or PhoneNumber is required");

            RuleFor(x => x.Email)
                .Cascade(CascadeMode.Stop)
                .EmailAddress().WithMessage("Email Address must be valid")
                .MaximumLength(256).WithMessage("Email must not exceed 256 characters")
                .When(x => !string.IsNullOrWhiteSpace(x.Email));

            // PhoneNumber validation
            RuleFor(x => x.PhoneNumber)
                .Matches(@"^(010|011|012|015)\d{8}$")
                .WithMessage("Phone number must be 11 digits and start with 010, 011, 012, or 015")
                .When(x => !string.IsNullOrWhiteSpace(x.PhoneNumber));

        }
    }

}
