using Clinic.API.API.Dtos.DoctorDtos;
using Clinic.API.API.Validators.ApplicationUserValidators;
using FluentValidation;

namespace Clinic.API.API.Validators.DoctorValidators
{

    public class UpdateDoctorDtoValidator : AbstractValidator<UpdateDoctorDto>
    {
        public UpdateDoctorDtoValidator()
        {
            RuleFor(x => x.DateOfBirth)
                .Cascade(CascadeMode.Stop)
                .Must(dob => dob <= DateTime.UtcNow.AddYears(-20))
                    .WithMessage("Doctor must be at least 20 years old")
                .Must(dob => dob >= DateTime.UtcNow.AddYears(-120))
                    .WithMessage("Doctor cannot be older than 120 years");

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

            // Specialty - optional, max length 100
            RuleFor(x => x.Specialty)
                .MaximumLength(100)
                .When(x => !string.IsNullOrWhiteSpace(x.Specialty))
                .WithMessage("Specialty length must not exceed 100 characters");

            // Years of Experience - optional, must be 0-50 if provided
            RuleFor(x => x.YearsOfExperience)
                .InclusiveBetween(0, 50)
                .When(x => x.YearsOfExperience.HasValue)
                .WithMessage("Years of experience must be between 0 and 50");

            // Nested UpdateApplicationUserDto - optional, validate if provided
            RuleFor(x => x.UpdateApplicationUserDto)
              .SetValidator(new UpdateApplicationUserDtoValidator()!)
              .When(x => x.UpdateApplicationUserDto != null);

            RuleFor(x => x)
                .Must(x =>
                    x.DateOfBirth.HasValue ||
                    !string.IsNullOrWhiteSpace(x.Gender) ||
                    !string.IsNullOrWhiteSpace(x.Address) ||
                    !string.IsNullOrWhiteSpace(x.Specialty) ||
                    x.YearsOfExperience.HasValue ||
                    x.UpdateApplicationUserDto != null)
                .WithMessage("At least one field must be provided for update");

        }
    }

}
