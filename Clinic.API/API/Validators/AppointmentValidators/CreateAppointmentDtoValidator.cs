using Clinic.API.API.Dtos.AppointmentDtos;
using FluentValidation;
namespace Clinic.API.API.Validators.AppointmentValidators
{
    public class CreateAppointmentDtoValidator : AbstractValidator<CreateAppointmentDto>
    {
        public CreateAppointmentDtoValidator()
        {
            RuleFor(x => x.AppointmentDate)
                .Cascade(CascadeMode.Stop)
                .NotEmpty().WithMessage("Appointment date is required")
                .GreaterThan(DateTime.UtcNow).WithMessage("Appointment date must be in the future");

            RuleFor(x => x.Notes)
             .MaximumLength(500)
             .When(x => !string.IsNullOrWhiteSpace(x.Notes))
             .WithMessage("Notes must not exceed 500 characters");

            RuleFor(x => x.PatientId)
                .NotEmpty().WithMessage("Patient Id is required");

            RuleFor(x => x.DoctorId)
                .NotEmpty().WithMessage("Doctor Id is required");
        }
    }
}