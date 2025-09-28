using Clinic.API.API.Dtos.PatientDtos;
using Clinic.API.API.Validators.ApplicationUserValidators;
using FluentValidation;

namespace Clinic.API.API.Validators.PatientValidators
{
    public class UpdatePatientDtoValidator : AbstractValidator<UpdatePatientDto>
    {
        public UpdatePatientDtoValidator()
        {

            RuleFor(x => x.DateOfBirth)
             .Cascade(CascadeMode.Stop)
             .Must(dob => dob < DateTime.UtcNow)
                 .WithMessage("Patient Date of birth must be a valid past date")
             .Must(dob => dob >= DateTime.UtcNow.AddYears(-120))
                 .WithMessage("Patient cannot be older than 120 years");


            RuleFor(x => x.Gender)
                .Cascade(CascadeMode.Stop)
                .MaximumLength(10).WithMessage("Gender length must not exceed 10 characters")
                .Matches("^(Male|Female|Other)$", System.Text.RegularExpressions.RegexOptions.IgnoreCase)
                .When(x => !string.IsNullOrWhiteSpace(x.Gender))
                .WithMessage("Gender must be Male, Female, or Other");

            // Address - optional, max length 200
            RuleFor(x => x.Address)
                .MaximumLength(200)
                .When(x => !string.IsNullOrWhiteSpace(x.Address))
                .WithMessage("Address length must not exceed 200 characters");

            RuleFor(x => x.UpdateApplicationUserDto)
                .SetValidator(new UpdateApplicationUserDtoValidator()!)
                .When(x => x.UpdateApplicationUserDto != null);

            RuleFor(x => x)
                .Must(x =>
                    x.DateOfBirth.HasValue ||
                    !string.IsNullOrWhiteSpace(x.Gender) ||
                    !string.IsNullOrWhiteSpace(x.Address) ||
                    x.UpdateApplicationUserDto != null)
                .WithMessage("At least one field must be provided for update");
        }
    }
}
