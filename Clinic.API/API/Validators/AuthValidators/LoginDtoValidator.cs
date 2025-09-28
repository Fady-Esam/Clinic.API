using Clinic.API.API.Dtos.AuthDtos;
using FluentValidation;

namespace Clinic.API.API.Validators.AuthValidators
{
    public class LoginDtoValidator : AbstractValidator<LoginDto>
    {
        public LoginDtoValidator()
        {

            RuleFor(x => x.UserName)
              .MaximumLength(256).WithMessage("Username must not exceed 256 characters")
              .When(x => !string.IsNullOrWhiteSpace(x.UserName));

            RuleFor(x => x.Email)
               .Cascade(CascadeMode.Stop)
               .EmailAddress().WithMessage("Email Address must be valid")
               .MaximumLength(256).WithMessage("Email must not exceed 256 characters")               
               .When(x => !string.IsNullOrWhiteSpace(x.Email));

            RuleFor(x => x.PhoneNumber)
                .Matches(@"^(010|011|012|015)\d{8}$")
                .WithMessage("Phone number must be 11 digits and start with 010, 011, 012, or 015")
                .When(x => !string.IsNullOrWhiteSpace(x.PhoneNumber));
            
            RuleFor(x => x.Password)
                .NotEmpty().WithMessage("Password is required");


            RuleFor(x => x)
                .Must(x =>
                    !string.IsNullOrWhiteSpace(x.UserName) ||
                    !string.IsNullOrWhiteSpace(x.Email) ||
                    !string.IsNullOrWhiteSpace(x.PhoneNumber))
                .WithMessage("You must provide Username, Email, or Phone Number to login.");
        }
    
    }

}
