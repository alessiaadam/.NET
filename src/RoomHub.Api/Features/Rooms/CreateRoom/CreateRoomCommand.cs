using RoomHub.Api.Common.Cqrs;
using RoomHub.Api.Domain.Common;
using RoomHub.Api.Domain.Rooms;

namespace RoomHub.Api.Features.Rooms.CreateRoom;

public sealed record CreateRoomCommand(string? Name, int? Capacity) : ICommand<Room>;

// Depends only on domain interfaces (no EF Core here).
public sealed class CreateRoomHandler(IRoomRepository rooms, IUnitOfWork unitOfWork)
    : ICommandHandler<CreateRoomCommand, Room>
{
    public async Task<Result<Room>> Handle(CreateRoomCommand command, CancellationToken cancellationToken)
    {
        var room = Room.Create(command.Name, command.Capacity);
        if (room.IsFailure)
            return room;

        rooms.Add(room.Value);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return room;
    }
}
