using RoomHub.Api.Services;

namespace RoomHub.Api.Features.Rooms.GetRoom;

public sealed record GetRoomResponse(Guid Id, string Name, int Capacity);

public static class GetRoomEndpoint
{
    public static RouteGroupBuilder MapGetRoom(this RouteGroupBuilder group)
    {
        group.MapGet("/{id:guid}", (Guid id, RoomService service) =>
        {
            var room = service.Get(id);
            return Results.Ok(new GetRoomResponse(room.Id, room.Name, room.Capacity));
        });

        return group;
    }
}
