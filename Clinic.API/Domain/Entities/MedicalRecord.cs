using System.ComponentModel.DataAnnotations;

namespace Clinic.API.Domain.Entities
{
    public class MedicalRecord
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        [MaxLength(4000)]
        public string ConsultationNotes { get; set; } = string.Empty;
        [MaxLength(1000)]
        public string Diagnosis { get; set; } = string.Empty;
        [MaxLength(2000)]
        public string TreatmentPlan { get; set; } = string.Empty;
        public DateTime RecordDate { get; set; }
        public Guid PatientId { get; set; }
        public Guid DoctorId { get; set; }
        public Guid AppointmentId { get; set; }
        public  Patient Patient { get; set; } = null!;
        public  Doctor Doctor { get; set; } = null!;
        public  Appointment Appointment { get; set; } = null!;
    }
}
