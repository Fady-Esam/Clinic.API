namespace Clinic.API.DL.Models
{
    public class ApiResponse<T>
    {
        public T? Data { get; set; }
        public string Message { get; set; } = string.Empty;
        public List<string> Errors { get; set; } = new();
        public int StatusCode { get; set; }


       private ApiResponse() { }

        // =================== Success Factory ===================
        public static ApiResponse<T> Success(T data, string message, int statusCode = 200)
        {
            return new ApiResponse<T>
            {
                Data = data,
                Message = message,
                StatusCode = statusCode
            };
        }

        // =================== Failure Factory ===================
        public static ApiResponse<T> Failure(string message, List<string> errors, int statusCode = 400)
        {
            return new ApiResponse<T>
            {
                Message = message,
                Errors = errors,
                StatusCode = statusCode
            };
        }

    }
}
