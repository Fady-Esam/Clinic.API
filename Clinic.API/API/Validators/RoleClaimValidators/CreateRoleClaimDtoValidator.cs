using Clinic.API.API.Dtos.RoleClaimDtos;
using FluentValidation;

namespace Clinic.API.API.Validators.RoleClaimValidators
{
    public class CreateRoleClaimDtoValidator : AbstractValidator<CreateRoleClaimDto>
    {
        public CreateRoleClaimDtoValidator()
        {
            // RoleId - required, not empty
            RuleFor(x => x.RoleId)
                .Cascade(CascadeMode.Stop)
                .NotEmpty().WithMessage("Role Id is required")
                .MaximumLength(450).WithMessage("Role Id length must not exceed 450 characters");

            // ClaimType - required, not empty
            RuleFor(x => x.ClaimType)
                .NotEmpty().WithMessage("Claim Type is required");

            // ClaimValue - required, not empty
            RuleFor(x => x.ClaimValue)
                .NotEmpty().WithMessage("Claim Value is required");
        }
    }
}
