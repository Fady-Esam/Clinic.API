namespace Clinic.API.API.Dtos.AuthDtos
{
    public class SmsConfirmDto
    {
        public string PhoneNumber { get; set; } = string.Empty;
        public string Code { get; set; } = string.Empty;
    }
}
