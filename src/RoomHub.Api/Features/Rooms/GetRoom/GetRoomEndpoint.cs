using Microsoft.AspNetCore.Http.HttpResults;
using RoomHub.Api.Common;
using RoomHub.Api.Domain.Rooms;
using RoomHub.Api.Repositories;

namespace RoomHub.Api.Features.Rooms.GetRoom;

public sealed record GetRoomResponse(Guid Id, string Name, int Capacity);

public static class GetRoomEndpoint
{
    public static RouteGroupBuilder MapGetRoom(this RouteGroupBuilder group)
    {
        group.MapGet("/{id:guid}", Handle);
        return group;
    }

    // Query (read-only): no command handler needed.
    private static Results<Ok<GetRoomResponse>, ProblemHttpResult> Handle(Guid id, RoomRepository rooms)
    {
        var room = rooms.Get(id);
        if (room is null)
            return RoomErrors.NotFound.ToProblem();

        return TypedResults.Ok(new GetRoomResponse(room.Id, room.Name, room.Capacity));
    }
}
