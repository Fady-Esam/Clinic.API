using Clinic.API.API.Dtos.RoleDtos;
using FluentValidation;

namespace Clinic.API.API.Validators.RoleValidators
{
    public class CreateRoleDtoValidator : AbstractValidator<CreateRoleDto>
    {
        public CreateRoleDtoValidator()
        {
            RuleFor(x => x.Name)
                .Cascade(CascadeMode.Stop)
                .NotEmpty().WithMessage("Role Name is required")
                .MaximumLength(256).WithMessage("Role Name must not exceed 256 characters");
        }
    }
}
