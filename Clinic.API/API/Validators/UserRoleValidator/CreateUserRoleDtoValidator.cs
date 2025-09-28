using Clinic.API.API.Dtos.UserRoleDtos;
using FluentValidation;

namespace Clinic.API.API.Validators.UserRoleValidator
{
    public class CreateUserRoleDtoValidator : AbstractValidator<CreateUserRoleDto>
    {
        public CreateUserRoleDtoValidator()
        {
            // UserIds - required, at least one item
            RuleFor(x => x.UserIds)
                .Cascade(CascadeMode.Stop)
                .NotNull().WithMessage("UserIds list is required")
                .Must(list => list.Count > 0).WithMessage("At least one UserId must be specified")
                .ForEach(idRule => idRule
                    .Cascade(CascadeMode.Stop)
                    .NotEmpty().WithMessage("UserId cannot be empty")
                    .MaximumLength(450).WithMessage("User Id length must not exceed 450 characters"));

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
