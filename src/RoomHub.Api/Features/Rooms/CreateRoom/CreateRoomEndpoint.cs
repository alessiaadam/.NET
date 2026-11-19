using RoomHub.Api.Services;

namespace RoomHub.Api.Features.Rooms.CreateRoom;

// Nullable fields: a missing field must reach our validation (R1) and give a ProblemDetails 400,
// not fail during JSON binding.
public sealed record CreateRoomRequest(string? Name, int? Capacity);

public sealed record CreateRoomResponse(Guid Id, string Name, int Capacity);

public static class CreateRoomEndpoint
{
    public static RouteGroupBuilder MapCreateRoom(this RouteGroupBuilder group)
    {
        group.MapPost("/", (CreateRoomRequest? request, RoomService service) =>
        {
            var room = service.Create(request?.Name, request?.Capacity);

            return Results.Created(
                $"/api/rooms/{room.Id}",
                new CreateRoomResponse(room.Id, room.Name, room.Capacity));
        });

        return group;
    }
}
