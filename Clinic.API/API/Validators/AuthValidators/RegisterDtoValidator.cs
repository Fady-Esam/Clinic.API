using Clinic.API.API.Dtos.AuthDtos;
using FluentValidation;

namespace Clinic.API.API.Validators.AuthValidators
{
    public class RegisterDtoValidator : AbstractValidator<RegisterDto>
    {
        private readonly string[] allowedEmailDomains = { "gmail.com", "outlook.com", "hotmail.com", "yahoo.com" };

        public RegisterDtoValidator()
        {
            RuleFor(x => x.UserName)
              .MaximumLength(256).WithMessage("Username must not exceed 256 characters")
              .When(x => !string.IsNullOrWhiteSpace(x.UserName));

            // Email validation
            RuleFor(x => x.Email)
                .Cascade(CascadeMode.Stop)
                .EmailAddress().WithMessage("Email Address must be valid")
                .MaximumLength(256).WithMessage("Email must not exceed 256 characters")
                .Must(BeAllowedEmailDomain).WithMessage("This email domain is not allowed")
                .When(x => !string.IsNullOrWhiteSpace(x.Email));

            // PhoneNumber validation
            RuleFor(x => x.PhoneNumber)
                .Matches(@"^(010|011|012|015)\d{8}$")
                .WithMessage("Phone number must be 11 digits and start with 010, 011, 012, or 015")
                .When(x => !string.IsNullOrWhiteSpace(x.PhoneNumber));

            // Password validation
            RuleFor(x => x.Password)
                .Cascade(CascadeMode.Stop)
                .NotEmpty().WithMessage("Password is required")
                .MinimumLength(8).WithMessage("Password must be at least 8 characters");

            // Ensure at least one identifier is provided
            RuleFor(x => x)
                .Must(x => !string.IsNullOrWhiteSpace(x.UserName)
                        || !string.IsNullOrWhiteSpace(x.Email)
                        || !string.IsNullOrWhiteSpace(x.PhoneNumber))
                .WithMessage("At least one of Username, Email, or Phone Number must be provided");

        }

        private bool BeAllowedEmailDomain(string? email)
        {
            if (string.IsNullOrWhiteSpace(email)) return false;
            var domain = email.Split('@').Last();
            return allowedEmailDomains.Contains(domain.ToLower());
        }
    }
}
