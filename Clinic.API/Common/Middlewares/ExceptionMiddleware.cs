using Clinic.API.Common.Exceptions;
using Clinic.API.Common.Responses;
using System.Text.Json;

namespace Clinic.API.Common.Middlewares
{
    public class ExceptionMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ExceptionMiddleware> _logger;


        public ExceptionMiddleware(RequestDelegate next, ILogger<ExceptionMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
               await _next(context); // continue pipeline
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unhandled exception occurred");
                await HandleExceptionAsync(context, ex);
            }
        }

        private static Task HandleExceptionAsync(HttpContext context, Exception ex)
        {
            context.Response.ContentType = "application/json";

            int statusCode;
            List<string> errors = new();
            string message = ex.Message;

            if (ex is ApiException apiEx)
            {
                statusCode = apiEx.StatusCode;
                errors = apiEx.Errors;
                if (!string.IsNullOrEmpty(apiEx.ExtraInfo))
                    message += $" | Context: {apiEx.ExtraInfo}";
            }
            else
            {
                statusCode = StatusCodes.Status500InternalServerError;
                errors.Add(ex.Message);
            }

            context.Response.StatusCode = statusCode;

            var response = ApiResponse<string>.Failure(message, errors, statusCode);

            var options = new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase };
            var json = JsonSerializer.Serialize(response, options);
            return context.Response.WriteAsync(json);
        }
    
    }

}
