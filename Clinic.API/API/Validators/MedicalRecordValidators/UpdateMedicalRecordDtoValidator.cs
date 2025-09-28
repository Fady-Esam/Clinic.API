using Clinic.API.API.Dtos.MedicalRecordDtos;
using FluentValidation;

namespace Clinic.API.API.Validators.MedicalRecordValidators
{
    public class UpdateMedicalRecordDtoValidator : AbstractValidator<UpdateMedicalRecordDto>
    {
        public UpdateMedicalRecordDtoValidator()
        {
            RuleFor(x => x)
              .Must(x =>
                  !string.IsNullOrWhiteSpace(x.ConsultationNotes) ||
                  !string.IsNullOrWhiteSpace(x.TreatmentPlan) ||
                  !string.IsNullOrWhiteSpace(x.Diagnosis))
              .WithMessage("At least one field must be provided for update");
        }
    }
}
