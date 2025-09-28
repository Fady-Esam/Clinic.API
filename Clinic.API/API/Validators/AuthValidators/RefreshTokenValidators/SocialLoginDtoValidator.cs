using Clinic.API.API.Dtos.AuthDtos;
using Clinic.API.API.Dtos.AuthDtos.RefreshTokenDtos;
using FluentValidation;

namespace Clinic.API.API.Validators.AuthValidators.RefreshTokenValidators
{
    public class SocialLoginDtoValidator : AbstractValidator<SocialLoginDto>
    {
        public SocialLoginDtoValidator()
        {
            RuleFor(x => x.Token)
                .NotEmpty().WithMessage("Token is required");
        }
    }
}
