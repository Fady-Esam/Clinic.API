using Clinic.API.API.Dtos.PatientDtos;
using FluentValidation;

namespace Clinic.API.API.Validators.PatientValidators
{

    public class CreatePatientDtoValidator : AbstractValidator<CreatePatientDto>
    {
        public CreatePatientDtoValidator()
        {

            RuleFor(x => x.DateOfBirth)
                .Cascade(CascadeMode.Stop)
                .NotEmpty()
                .WithMessage("Date of birth is required")
                .Must(dob => dob < DateTime.UtcNow)
                    .WithMessage("Patient Date of birth must be a valid past date")
                .Must(dob => dob >= DateTime.UtcNow.AddYears(-120))
                    .WithMessage("Patient cannot be older than 120 years");
            // Gender - required, max length 10, must be valid
            RuleFor(x => x.Gender)
                .Cascade(CascadeMode.Stop)
                .NotEmpty().WithMessage("Gender is required")
                .MaximumLength(10).WithMessage("Gender length must not exceed 10 characters")
                .Matches("^(Male|Female|Other)$", System.Text.RegularExpressions.RegexOptions.IgnoreCase)
                .WithMessage("Gender must be Male, Female, or Other");

            // Address - required, max length 200
            RuleFor(x => x.Address)
                .Cascade(CascadeMode.Stop)
                .NotEmpty().WithMessage("Address is required")
                .MaximumLength(200).WithMessage("Address length must not exceed 200 characters");

            // ApplicationUserId - required, not empty
            RuleFor(x => x.ApplicationUserId)
                .Cascade(CascadeMode.Stop)
                .NotEmpty().WithMessage("User Id is required")
                .MaximumLength(450).WithMessage("User Id length must not exceed 450 characters");
        }
    }
}
