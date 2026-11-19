using RoomHub.Api.Domain.Rooms;

namespace RoomHub.Api.Repositories;

// Still in-memory (level B). Replaced by EF Core + SQLite in level C.
public class RoomRepository
{
    private readonly List<Room> _rooms = new();
    private readonly object _lock = new();

    public void Add(Room room)
    {
        lock (_lock)
        {
            _rooms.Add(room);
        }
    }

    public Room? Get(Guid id)
    {
        lock (_lock)
        {
            return _rooms.FirstOrDefault(r => r.Id == id);
        }
    }
}
