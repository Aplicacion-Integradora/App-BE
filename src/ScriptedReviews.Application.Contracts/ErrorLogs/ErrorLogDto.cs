using System;

namespace ScriptedReviews.ErrorLogs;

public class ErrorLogDto
{
    public int Id { get; set; }
    public string Level { get; set; }
    public string Message { get; set; }
    public string? Exception { get; set; }
    public string? Source { get; set; }
    public string? Endpoint { get; set; }
    public string? HttpMethod { get; set; }
    public int? HttpStatusCode { get; set; }
    public string? UserId { get; set; }
    public string? IpAddress { get; set; }
    public DateTime CreatedAt { get; set; }
}
