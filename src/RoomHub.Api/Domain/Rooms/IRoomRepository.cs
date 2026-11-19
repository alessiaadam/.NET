namespace RoomHub.Api.Domain.Rooms;

public interface IRoomRepository
{
    Task<Room?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);

    // Does NOT save: IUnitOfWork.SaveChangesAsync does.
    void Add(Room room);
}
