using Clinic.API.API.Dtos.UserRoleDtos;
using FluentValidation;

namespace Clinic.API.API.Validators.UserRoleValidator
{
    public class UpdateUserRoleDtoValidator : AbstractValidator<UpdateUserRoleDto>
    {
        public UpdateUserRoleDtoValidator()
        {
            // UserId - required
            RuleFor(x => x.UserId)
                .Cascade(CascadeMode.Stop)
                .NotEmpty().WithMessage("UserId is required")
                .MaximumLength(450).WithMessage("UserId length must not exceed 450 characters");

            // RoleIds - required, at least one item
            RuleFor(x => x.RoleIds)
                .Cascade(CascadeMode.Stop)
                .NotNull().WithMessage("RoleIds list is required")
                .Must(list => list.Count > 0).WithMessage("At least one RoleId must be specified")
                .ForEach(idRule => idRule
                    .Cascade(CascadeMode.Stop)
                    .NotEmpty().WithMessage("RoleId cannot be empty")
                    .MaximumLength(450).WithMessage("Role Id length must not exceed 450 characters"));
        }
    }
}
