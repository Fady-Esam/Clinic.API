namespace Clinic.API.DL.Models
{
    public class ApiException : Exception
    {
        public int StatusCode { get; }
        public List<string> Errors { get; } = new();
        public string? ExtraInfo { get; }

        public ApiException(string message,
                            int statusCode = StatusCodes.Status400BadRequest,
                            List<string>? errors = null,
                            string? extraInfo = null) : base(message)
        {
            StatusCode = statusCode;
            if (errors != null) Errors = errors;
            ExtraInfo = extraInfo;
        }
        
    }
}
