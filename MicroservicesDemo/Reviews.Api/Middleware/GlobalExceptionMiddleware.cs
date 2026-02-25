using Reviews.Api.CustomExceptions;
using System.Net;
using System.Text.Json;

namespace Reviews.Api.Middleware
{
    public class GlobalExceptionMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<GlobalExceptionMiddleware> _logger;

        public GlobalExceptionMiddleware(
            RequestDelegate next,
            ILogger<GlobalExceptionMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task Invoke(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (BusinessException ex)
            {
                await HandleExceptionAsync(context, ex, "Ocurrió un error de negocio");
            }
            catch (TransactionException ex)
            {
                await HandleExceptionAsync(context, ex, "Ocurrió un error de transaccion");
            }
            catch (Exception ex)
            {
                await HandleExceptionAsync(context, ex, "Ocurrió un error inesperado");
            }
            //catch (Exception ex)
            //{

            //    throw; //No se pierde stacktrace original, no se ocultan errores

            //    //throw ex; //Se sobreescribe el stacktrace original, se ocultan errores
            //}
        }

        private Task HandleExceptionAsync(HttpContext context, Exception exception, string description)
        {
            context.Response.ContentType = "application/json";
            context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;

            var response = new
            {
                error = description,
                detail = exception.Message,
            };

            var jsonResponse = JsonSerializer.Serialize(response);

            _logger.LogError(exception, description);

            return context.Response.WriteAsync(jsonResponse);
        }
    }
}
