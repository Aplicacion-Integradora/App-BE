using System;
using Volo.Abp.Domain.Entities;

namespace ScriptedReviews.ErrorLogs;

public class ErrorLog : AggregateRoot<int>
{
    public string Level { get; private set; }        // Errores, Warnings, etc.
    public string Message { get; private set; }      // Mensaje de error
    public string? Exception { get; private set; }   // Stackea trace si está disponible
    public string? Source { get; private set; }      // Clase/método donde ocurrió el error 
    public string? Endpoint { get; private set; }    // API endpoint si es aplicable
    public string? HttpMethod { get; private set; }  // GET, POST, etc.
    public int? HttpStatusCode { get; private set; } // Código de estado de respuesta
    public string? UserId { get; private set; }      // Usuario que provocó el error
    public string? IpAddress { get; private set; }   // IP del usuario
    public DateTime CreatedAt { get; private set; }  // Fecha y hora de creación

    private ErrorLog() { }

    public ErrorLog(
        string level,
        string message,
        string? exception = null,
        string? source = null,
        string? endpoint = null,
        string? httpMethod = null,
        int? httpStatusCode = null,
        string? userId = null,
        string? ipAddress = null)
    {
        Level = level;
        Message = message;
        Exception = exception;
        Source = source;
        Endpoint = endpoint;
        HttpMethod = httpMethod;
        HttpStatusCode = httpStatusCode;
        UserId = userId;
        IpAddress = ipAddress;
        CreatedAt = DateTime.UtcNow;
    }
}
