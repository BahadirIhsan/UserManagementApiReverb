namespace UserManagementApiReverb.BusinessLayer.DTOs.LogDto_s;

public record LogEntry(
    DateTime Timestamp,
    string Level,
    string Category,
    string EventType,
    string Operation,
    string Message,
    string? UserId
    );