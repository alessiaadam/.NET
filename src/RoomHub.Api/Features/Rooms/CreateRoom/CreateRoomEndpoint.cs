using Microsoft.AspNetCore.Http.HttpResults;
using RoomHub.Api.Common;
using RoomHub.Api.Common.Cqrs;
using RoomHub.Api.Domain.Rooms;

namespace RoomHub.Api.Features.Rooms.CreateRoom;

// Nullable fields: a missing field reaches the domain (R1) and gives a ProblemDetails 400.
public sealed record CreateRoomRequest(string? Name, int? Capacity);

public sealed record CreateRoomResponse(Guid Id, string Name, int Capacity);

public static class CreateRoomEndpoint
{
    public static RouteGroupBuilder MapCreateRoom(this RouteGroupBuilder group)
    {
        group.MapPost("/", Handle);
        return group;
    }

    private static async Task<Results<Created<CreateRoomResponse>, ProblemHttpResult>> Handle(
        CreateRoomRequest? request,
        ICommandHandler<CreateRoomCommand, Room> handler,
        CancellationToken cancellationToken)
    {
        var result = await handler.Handle(
            new CreateRoomCommand(request?.Name, request?.Capacity), cancellationToken);

        if (result.IsFailure)
            return result.Error!.ToProblem();

        var room = result.Value;
        return TypedResults.Created(
            $"/api/rooms/{room.Id}",
            new CreateRoomResponse(room.Id, room.Name, room.Capacity));
    }
}
