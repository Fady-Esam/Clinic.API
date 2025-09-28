using Clinic.API.API.Dtos.UserClaimDtos;
using FluentValidation;

namespace Clinic.API.API.Validators.UserClaimValidator
{

    public class CreateUserClaimDtoValidator : AbstractValidator<CreateUserClaimDto>
    {
        public CreateUserClaimDtoValidator()
        {
            // UserId - required
            RuleFor(x => x.UserId)
                .Cascade(CascadeMode.Stop)
                .NotEmpty().WithMessage("User Id is required")
                .MaximumLength(450).WithMessage("User Id length must not exceed 450 characters");

            // ClaimType - required
            RuleFor(x => x.ClaimType)
                .NotEmpty().WithMessage("Claim Type is required");

            // ClaimValue - required
            RuleFor(x => x.ClaimValue)
                .Cascade(CascadeMode.Stop)
                .NotEmpty().WithMessage("Claim Value is required");
        }
    }
}
