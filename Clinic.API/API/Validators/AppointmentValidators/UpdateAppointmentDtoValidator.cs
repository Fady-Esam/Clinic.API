using Clinic.API.API.Dtos.AppointmentDtos;
using FluentValidation;

namespace Clinic.API.API.Validators.AppointmentValidators
{
    public class UpdateAppointmentDtoValidator : AbstractValidator<UpdateAppointmentDto>
    {
        public UpdateAppointmentDtoValidator()
        {
            RuleFor(x => x.AppointmentDate)
                    .GreaterThan(DateTime.UtcNow).WithMessage("Appointment date must be in the future");

            RuleFor(x => x.Status)
                .IsInEnum()
                .When(x => x.Status.HasValue)
                .WithMessage("Invalid appointment status");

            RuleFor(x => x.Notes)
                .MaximumLength(500)
                .When(x => !string.IsNullOrWhiteSpace(x.Notes))
                .WithMessage("Notes must not exceed 500 characters");

            RuleFor(x => x.PatientId)
                .Must(id => !id.HasValue || id.Value != Guid.Empty)
                .WithMessage("Patient Id must be valid");

            RuleFor(x => x.DoctorId)
                .Must(id => !id.HasValue || id.Value != Guid.Empty)
                .WithMessage("Doctor Id must be valid");

            RuleFor(x => x)
                .Must(x =>
                    x.AppointmentDate.HasValue ||
                    x.Status.HasValue ||
                    !string.IsNullOrWhiteSpace(x.Notes) ||
                    x.PatientId.HasValue ||
                    x.DoctorId.HasValue
                )
                .WithMessage("At least one field must be provided to update the appointment.");
        }
    }

}
