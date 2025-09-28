
using Clinic.API.API.Dtos.AuthDtos.PasswordDtos;
using FluentValidation;

namespace Clinic.API.API.Validators.AuthValidators.PasswordValidators
{

    public class ChangePasswordDtoValidator : AbstractValidator<ChangePasswordDto>
    {
        public ChangePasswordDtoValidator()
        {


            RuleFor(x => x.UserId)
                .Cascade(CascadeMode.Stop)
                .NotEmpty().WithMessage("User Id is required")
                .MaximumLength(450).WithMessage("User Id length must not exceed 450 characters");

            RuleFor(x => x.CurrentPassword)
                .NotEmpty().WithMessage("Current password is required");


            RuleFor(x => x.NewPassword)
                .Cascade(CascadeMode.Stop)
                .NotEmpty().WithMessage("New password is required")
                .MinimumLength(8).WithMessage("Password must be at least 8 characters");


            RuleFor(x => x)
                .Must(x => x.CurrentPassword != x.NewPassword)
                .WithMessage("New password must be different from the current password");
        }
    }
}
