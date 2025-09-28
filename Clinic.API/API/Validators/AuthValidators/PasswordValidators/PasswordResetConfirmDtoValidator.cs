using Clinic.API.API.Dtos.AuthDtos.PasswordDtos;

namespace Clinic.API.API.Validators.AuthValidators.PasswordValidators
{
    using FluentValidation;

    public class PasswordResetConfirmDtoValidator : AbstractValidator<PasswordResetConfirmDto>
    {
        public PasswordResetConfirmDtoValidator()
        {
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

            // Code validation
            RuleFor(x => x.Code)
                .Cascade(CascadeMode.Stop)
                .NotEmpty().WithMessage("Reset code is required")
                .Length(6).WithMessage("Reset code must be 6 digits")
                .Matches("^[0-9]{6}$").WithMessage("Reset code must contain only digits");

            // New password validation
            RuleFor(x => x.NewPassword)
                .Cascade(CascadeMode.Stop)
                .NotEmpty().WithMessage("New password is required")
                .MinimumLength(8).WithMessage("Password must be at least 8 characters");

        }
    }

}
