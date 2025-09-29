using System.Net;
using System.Text.Json;

namespace teste_pratico.Middleware
{
    /// <summary>
    /// Middleware para tratamento global de exceções
    /// Captura exceções não tratadas e converte em respostas HTTP apropriadas
    /// </summary>
    public class ExceptionHandlerMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ExceptionHandlerMiddleware> _logger;
        private readonly IWebHostEnvironment _environment;

        public ExceptionHandlerMiddleware(
            RequestDelegate next,
            ILogger<ExceptionHandlerMiddleware> logger,
            IWebHostEnvironment environment)
        {
            _next = next ?? throw new ArgumentNullException(nameof(next));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _environment = environment ?? throw new ArgumentNullException(nameof(environment));
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Exceção não tratada capturada pelo middleware: {ExceptionType}",
                    ex.GetType().Name);

                await HandleExceptionAsync(context, ex);
            }
        }

        private async Task HandleExceptionAsync(HttpContext context, Exception exception)
        {
            // Garantir que a resposta não foi iniciada
            if (context.Response.HasStarted)
            {
                _logger.LogWarning("Não é possível modificar a resposta, ela já foi iniciada");
                return;
            }

            var response = context.Response;
            response.ContentType = "application/json";

            var errorResponse = CreateErrorResponse(exception);
            response.StatusCode = (int)errorResponse.StatusCode;

            var jsonResponse = JsonSerializer.Serialize(errorResponse.ErrorDetails, new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                WriteIndented = true
            });

            await response.WriteAsync(jsonResponse);
        }

        private ErrorResponseModel CreateErrorResponse(Exception exception)
        {
            return exception switch
            {
                ArgumentException argEx => new ErrorResponseModel
                {
                    StatusCode = HttpStatusCode.BadRequest,
                    ErrorDetails = new
                    {
                        error = "Dados inválidos",
                        message = argEx.Message,
                        type = "validation_error"
                    }
                },

                InvalidOperationException invalidOpEx => new ErrorResponseModel
                {
                    StatusCode = HttpStatusCode.BadRequest,
                    ErrorDetails = new
                    {
                        error = "Operação inválida",
                        message = invalidOpEx.Message,
                        type = "business_rule_error"
                    }
                },

                HttpRequestException httpEx => new ErrorResponseModel
                {
                    StatusCode = HttpStatusCode.BadRequest,
                    ErrorDetails = new
                    {
                        error = "Erro de comunicação externa",
                        message = "Erro ao processar requisição. Tente novamente.",
                        type = "external_service_error",
                        details = _environment.IsDevelopment() ? httpEx.Message : null
                    }
                },

                TaskCanceledException timeoutEx when timeoutEx.InnerException is TimeoutException => new ErrorResponseModel
                {
                    StatusCode = HttpStatusCode.RequestTimeout,
                    ErrorDetails = new
                    {
                        error = "Timeout",
                        message = "A operação excedeu o tempo limite.",
                        type = "timeout_error"
                    }
                },

                _ => new ErrorResponseModel
                {
                    StatusCode = HttpStatusCode.InternalServerError,
                    ErrorDetails = new
                    {
                        error = "Erro interno do servidor",
                        message = "Ocorreu um erro inesperado. Tente novamente mais tarde.",
                        type = "internal_error",
                        details = _environment.IsDevelopment() ? exception.Message : null,
                        stackTrace = _environment.IsDevelopment() ? exception.StackTrace : null
                    }
                }
            };
        }

        /// <summary>
        /// Modelo interno para resposta de erro
        /// </summary>
        private class ErrorResponseModel
        {
            public HttpStatusCode StatusCode { get; set; }
            public object ErrorDetails { get; set; } = new();
        }
    }

    /// <summary>
    /// Extensão para registrar o middleware
    /// </summary>
    public static class ExceptionHandlerMiddlewareExtensions
    {
        /// <summary>
        /// Adiciona o middleware de tratamento de exceções ao pipeline
        /// </summary>
        /// <param name="builder">O builder da aplicação</param>
        /// <returns>O builder para encadeamento</returns>
        public static IApplicationBuilder UseGlobalExceptionHandler(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<ExceptionHandlerMiddleware>();
        }
    }
}