namespace RoomHub.Api.Domain.Common;

public enum ErrorType
{
    Validation,
    NotFound,
    Conflict
}

// A business error is a value, not an exception.
public sealed record Error(string Code, string Description, ErrorType Type)
{
    public static Error Validation(string code, string description) => new(code, description, ErrorType.Validation);
    public static Error NotFound(string code, string description) => new(code, description, ErrorType.NotFound);
    public static Error Conflict(string code, string description) => new(code, description, ErrorType.Conflict);
}
