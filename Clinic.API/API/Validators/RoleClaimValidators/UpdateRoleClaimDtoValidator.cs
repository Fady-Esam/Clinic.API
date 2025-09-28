namespace Clinic.API.API.Validators.RoleClaimValidators
{
    using Clinic.API.API.Dtos.RoleClaimDtos;
    using FluentValidation;

    public class UpdateRoleClaimDtoValidator : AbstractValidator<UpdateRoleClaimDto>
    {
        public UpdateRoleClaimDtoValidator()
        {

            // Ensure at least one field is provided
            RuleFor(x => x)
                .Must(x => !string.IsNullOrWhiteSpace(x.ClaimType)
                        || !string.IsNullOrWhiteSpace(x.ClaimValue))
                .WithMessage("At least one of ClaimType or ClaimValue must be provided for update");
        }
    }

}
