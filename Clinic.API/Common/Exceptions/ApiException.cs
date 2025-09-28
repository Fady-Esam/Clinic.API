namespace Clinic.API.Common.Exceptions
{
    public class ApiException : Exception
    {
        public int StatusCode { get; }
        public List<string> Errors { get; } = new();
        public string? ExtraInfo { get; }

        public ApiException(string message,
                            List<string> errors,
                            int statusCode = StatusCodes.Status400BadRequest,
                            string? extraInfo = null) : base(message)
        {
            StatusCode = statusCode;
            Errors = errors;
            ExtraInfo = extraInfo;
        }
        
    }
}
