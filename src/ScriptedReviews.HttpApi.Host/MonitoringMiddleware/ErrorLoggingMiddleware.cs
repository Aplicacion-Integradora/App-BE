using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using ScriptedReviews.ErrorLogs;
using System;
using System.Threading.Tasks;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.Users;

namespace ScriptedReviews.MonitoringMiddleware;


/// Middleware para capturar y registrar errores/advertencias en la base de datos.

public class ErrorLoggingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ErrorLoggingMiddleware> _logger;

    public ErrorLoggingMiddleware(RequestDelegate next, ILogger<ErrorLoggingMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task Invoke(
        HttpContext context, 
        IRepository<ErrorLog, int> errorLogRepository,
        ICurrentUser currentUser)
    {
        try
        {
            await _next(context);

            // Log errores HTTP (4xx, 5xx)
            if (context.Response.StatusCode >= 400)
            {
                var errorLog = new ErrorLog(
                    level: context.Response.StatusCode >= 500 ? "Error" : "Warning",
                    message: $"HTTP {context.Response.StatusCode} response",
                    exception: null,
                    source: "HttpResponse",
                    endpoint: context.Request.Path,
                    httpMethod: context.Request.Method,
                    httpStatusCode: context.Response.StatusCode,
                    userId: currentUser.Id?.ToString(),
                    ipAddress: context.Connection.RemoteIpAddress?.ToString()
                );

                try
                {
                    await errorLogRepository.InsertAsync(errorLog);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error al guardar log de error HTTP.");
                }
            }
        }
        catch (Exception ex)
        {
            // Log excepciones no manejadas
            _logger.LogError(ex, "Excepción no manejada en la solicitud.");

            var errorLog = new ErrorLog(
                level: "Error",
                message: ex.Message,
                exception: ex.ToString(),
                source: ex.Source,
                endpoint: context.Request.Path,
                httpMethod: context.Request.Method,
                httpStatusCode: 500,
                userId: currentUser.Id?.ToString(),
                ipAddress: context.Connection.RemoteIpAddress?.ToString()
            );

            try
            {
                await errorLogRepository.InsertAsync(errorLog);
            }
            catch (Exception innerEx)
            {
                _logger.LogError(innerEx, "Error al guardar log de excepción.");
            }

            // Re-lanza para que el manejador global pueda manejarlo
            throw;
        }
    }
}
