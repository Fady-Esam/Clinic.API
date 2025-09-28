using Clinic.API.API.Dtos.AppointmentDtos;
using Clinic.API.API.Dtos.DoctorDtos;
using Clinic.API.API.Dtos.PatientDtos;
using Clinic.API.Domain.Entities;

namespace Clinic.API.API.Dtos.MedicalRecordDtos
{
    public class MedicalRecordDto
    {
        public Guid Id { get; set; }
        public PatientDto Patient { get; set; } = null!;
        public DoctorDto Doctor { get; set; } = null!;
        public AppointmentDto Appointment { get; set; } = null!;
        public string ConsultationNotes { get; set; } = string.Empty;
        public string Diagnosis { get; set; } = string.Empty;
        public string TreatmentPlan { get; set; } = string.Empty;
        public DateTime RecordDate { get; set; }
    }
}
