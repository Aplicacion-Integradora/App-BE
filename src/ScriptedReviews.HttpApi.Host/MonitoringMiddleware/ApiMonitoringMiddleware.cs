using Microsoft.AspNetCore.Http;
using System.Diagnostics;
using System.Threading.Tasks;
using ScriptedReviews.MonitoringLogs;
using Volo.Abp.Domain.Repositories;
using Microsoft.Extensions.Logging;
using System;

namespace ScriptedReviews.MonitoringMiddleware;

    public class ApiMonitoringMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ApiMonitoringMiddleware> _logger;

        public ApiMonitoringMiddleware(RequestDelegate next, ILogger<ApiMonitoringMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task Invoke(HttpContext context, IRepository<ApiMonitoringLog, int> _apiMonitoringRepository)
        {
        var path = context.Request.Path.Value?.ToLower();
        var method = context.Request.Method.ToUpper();

        // --- FILTRO DE RUIDO (EXCLUSIONES) ---
        if (path != null && (
            // 1. AUTO-MONITOREO (Para no contar cuando miras el panel)
            path.Contains("api-monitoring") ||
            path.Contains("error-log") ||
            path.Contains("application-configuration") || // Configuración
            path.Contains("api-definition") ||            // Definición de API 
            path.Contains("_configuration") ||            // Configuración de Auth 
            path.Contains("swagger") ||
            path.Contains("favicon") ||
            method == "OPTIONS"
        ))
        {
            await _next(context);
            return;
        }
        var stopwatch = Stopwatch.StartNew();

            try
            {
                await _next(context);
            }
            finally
            {
                stopwatch.Stop();
                var log = new ApiMonitoringLog(
                    context.Request.Path,
                    context.Request.Method,
                    stopwatch.ElapsedMilliseconds,
                    context.Response.StatusCode,
                    context.Request.Headers["User-Agent"].ToString(),
                    context.Connection.RemoteIpAddress?.ToString()
                );

                try
                {
                    await _apiMonitoringRepository.InsertAsync(log);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error al registrar el monitoreo de API.");
                }
            }
        }
    }

