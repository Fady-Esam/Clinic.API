namespace Clinic.API.DL.Models
{
    public class ApiResponse<T>
    {
        public T Data { get; set; }
        public string Message { get; set; }
        public List<string> Errors { get; set; } = new();
        public int StatusCode { get; set; }

    }
}
