using Clinic.API.API.Dtos.MedicalRecordDtos;
using FluentValidation;

namespace Clinic.API.API.Validators.MedicalRecordValidators
{
    public class CreateMedicalRecordDtoValidator : AbstractValidator<CreateMedicalRecordDto>
    {
        public CreateMedicalRecordDtoValidator()
        {
            RuleFor(p => p.PatientId)
                .NotEmpty().WithMessage("PatientId is required.");

            RuleFor(p => p.DoctorId)
                .NotEmpty().WithMessage("DoctorId is required.");

            RuleFor(p => p.AppointmentId)
                .NotEmpty().WithMessage("DoctorId is required.");

            RuleFor(p => p.ConsultationNotes)
                .NotEmpty().WithMessage("ConsultationNotes is required.")
                .MaximumLength(4000).WithMessage("ConsultationNotes must not exceed 4000 characters.");

            RuleFor(p => p.Diagnosis)
                .NotEmpty().WithMessage("Diagnosis is required.")
                .MaximumLength(1000).WithMessage("ConsultationNotes must not exceed 1000 characters.");

            RuleFor(p => p.TreatmentPlan)
                .MaximumLength(2000).WithMessage("TreatmentPlan must not exceed 2000 characters.");
        }
    }
}
