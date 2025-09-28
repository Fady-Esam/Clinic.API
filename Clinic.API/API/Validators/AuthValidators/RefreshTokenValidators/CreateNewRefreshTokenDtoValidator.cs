using Clinic.API.API.Dtos.AuthDtos.RefreshTokenDtos;
using FluentValidation;

namespace Clinic.API.API.Validators.AuthValidators.RefreshTokenValidators
{
    public class CreateNewRefreshTokenDtoValidator : AbstractValidator<CreateNewRefreshTokenDto>
    {
        public CreateNewRefreshTokenDtoValidator()
        {
            RuleFor(x => x.Token)
                .NotEmpty().WithMessage("Token is required");
        }
    }
}
