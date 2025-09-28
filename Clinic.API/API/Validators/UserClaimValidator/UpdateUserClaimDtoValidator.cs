using Clinic.API.API.Dtos.UserClaimDtos;
using FluentValidation;

namespace Clinic.API.API.Validators.UserClaimValidator
{

    public class UpdateUserClaimDtoValidator : AbstractValidator<UpdateUserClaimDto>
    {
        public UpdateUserClaimDtoValidator()
        {
            RuleFor(x => x)
                .Must(x => !string.IsNullOrWhiteSpace(x.ClaimType)
                        || !string.IsNullOrWhiteSpace(x.ClaimValue))
                .WithMessage("At least one of ClaimType or ClaimValue must be provided for update");
        }
    }
}
