namespace Clinic.API.API.Dtos.MedicalRecordDtos
{
    public class CreateMedicalRecordDto
    {
        public Guid? PatientId { get; set; }
        public Guid? DoctorId { get; set; }
        public Guid? AppointmentId { get; set; }
        public string ConsultationNotes { get; set; } = string.Empty;
        public string Diagnosis { get; set; } = string.Empty;
        public string TreatmentPlan { get; set; } = string.Empty;
    }
}
