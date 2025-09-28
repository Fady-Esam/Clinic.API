using Clinic.API.API.Dtos.DoctorDtos;
using FluentValidation;

namespace Clinic.API.API.Validators.DoctorValidators
{

    public class CreateDoctorDtoValidator : AbstractValidator<CreateDoctorDto>
    {
        public CreateDoctorDtoValidator()
        {
            // Date of Birth
            RuleFor(x => x.DateOfBirth)
                .Cascade(CascadeMode.Stop)
                .NotEmpty().WithMessage("Date of birth is required")
                .Must(dob => dob <= DateTime.UtcNow.AddYears(-20))
                    .WithMessage("Doctor must be at least 20 years old")
                .Must(dob => dob >= DateTime.UtcNow.AddYears(-120))
                    .WithMessage("Doctor cannot be older than 120 years");

            // Gender
            RuleFor(x => x.Gender)
                .Cascade(CascadeMode.Stop)
                .NotEmpty().WithMessage("Gender is required")
                .MaximumLength(10).WithMessage("Gender length must not exceed 10 characters")
                .Matches("^(Male|Female|Other)?$", System.Text.RegularExpressions.RegexOptions.IgnoreCase)
                .When(x => !string.IsNullOrWhiteSpace(x.Gender))
                .WithMessage("Gender must be Male, Female, or Other");

            // Address
            RuleFor(x => x.Address)
                .MaximumLength(200).WithMessage("Address length must not exceed 200 characters");

            // Specialty
            RuleFor(x => x.Specialty)
                .Cascade(CascadeMode.Stop)
                .NotEmpty().WithMessage("Specialty is required")
                .MaximumLength(100).WithMessage("Specialty length must not exceed 100 characters");

            // Years of Experience
            RuleFor(x => x.YearsOfExperience)
                .Cascade(CascadeMode.Stop)
                .NotEmpty().WithMessage("YearsOfExperience is required")
                .InclusiveBetween(0, 50).WithMessage("Years of experience must be between 0 and 50");

            // Application User Id
            RuleFor(x => x.ApplicationUserId)
                .Cascade(CascadeMode.Stop)
                .NotEmpty().WithMessage("User Id is required")
                .MaximumLength(450).WithMessage("User Id length must not exceed 450 characters");
        }
    }
}
