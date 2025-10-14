using ECommerceBusinessLogic;
using System.Text.Json;

namespace ECommwerceWebAPI.Middlewares
{
    public class ExceptionMiddleware
    {
        private readonly RequestDelegate next;
        private readonly ILogger<ExceptionMiddleware> logger;
        public ExceptionMiddleware(RequestDelegate next, ILogger<ExceptionMiddleware> logger)
        {
            this.next = next;
            this.logger = logger;
        }

        public async Task InvokeAsync(HttpContext httpContext)
        {
            try
            {
                await next(httpContext);
            }
            catch (Exception ex)
            {
                await HandleExceptionAsync(httpContext, ex);
            }
        }

        private async Task HandleExceptionAsync(HttpContext context, Exception exception)
        {
        // Set response status code and content type
            context.Response.ContentType = "application/json";
        context.Response.StatusCode = StatusCodes.Status500InternalServerError;

            var (statusCode, errorResponse) = exception switch
            {
                BusinessException be => (
                    StatusCodes.Status400BadRequest,
                    new ErrorResponse { Error = be.Message, ErrorCode = be.ErrorCode }
                ),
                _ => (
                    StatusCodes.Status500InternalServerError,
                    new ErrorResponse { Error = "An unexpected error occurred.", ErrorCode = "INTERNAL_SERVER_ERROR" }
                )
            };

            // Log all exceptions with details (safe since this is server-side)
            if (exception is BusinessException businessEx)
            {
                logger.LogInformation("Business exception: {Message}, Code: {ErrorCode}", businessEx.Message, businessEx.ErrorCode);
            }
            else
            {
                logger.LogError(exception, "Unexpected error occurred.");
            }

            context.Response.StatusCode = statusCode;
            await context.Response.WriteAsync(JsonSerializer.Serialize(errorResponse));
        }
    }

    public class ErrorResponse
    {
        public string Error { get; set; }
        public string ErrorCode { get; set; }
    }
}
