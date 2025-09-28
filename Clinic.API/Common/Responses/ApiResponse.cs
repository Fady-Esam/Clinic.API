namespace Clinic.API.Common.Responses
{
    public class ApiResponse<T>
    {
        public bool IsSuccess { get; set; }
        public T? Data { get; set; }
        public string Message { get; set; } = string.Empty;
        public List<string> Errors { get; set; } = new();
        public int StatusCode { get; set; }


       private ApiResponse() { }

        // =================== Success Factory ===================
        public static ApiResponse<T> Success(T data, string message, int statusCode = StatusCodes.Status200OK)
        {
            return new ApiResponse<T>
            {
                IsSuccess = true,
                Data = data,
                Message = message,
                StatusCode = statusCode
            };
        }

        public static ApiResponse<object> SuccessNoData(string message, int statusCode = StatusCodes.Status204NoContent)
        {
            return new ApiResponse<object>
            {
                IsSuccess = true,
                Message = message,
                StatusCode = statusCode
            };
        }


        // =================== Failure Factory ===================
        public static ApiResponse<T> Failure(string message, List<string> errors, int statusCode = StatusCodes.Status400BadRequest)
        {
            return new ApiResponse<T>
            {
                IsSuccess = false,
                Message = message,
                Errors = errors,
                StatusCode = statusCode
            };
        }

    }

}
