using RoomHub.Api.Common.Cqrs;
using RoomHub.Api.Domain.Common;
using RoomHub.Api.Domain.Rooms;
using RoomHub.Api.Repositories;

namespace RoomHub.Api.Features.Rooms.CreateRoom;

public sealed record CreateRoomCommand(string? Name, int? Capacity) : ICommand<Room>;

public sealed class CreateRoomHandler(RoomRepository rooms) : ICommandHandler<CreateRoomCommand, Room>
{
    public Task<Result<Room>> Handle(CreateRoomCommand command, CancellationToken cancellationToken)
    {
        var room = Room.Create(command.Name, command.Capacity);
        if (room.IsSuccess)
            rooms.Add(room.Value);

        return Task.FromResult(room);
    }
}
