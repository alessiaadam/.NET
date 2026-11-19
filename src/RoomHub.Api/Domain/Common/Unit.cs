namespace RoomHub.Api.Domain.Common;

// "No value" for operations that only succeed or fail (e.g. Result<Unit> for Cancel).
public readonly record struct Unit
{
    public static readonly Unit Value = new();
}
