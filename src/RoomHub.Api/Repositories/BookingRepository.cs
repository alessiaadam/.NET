using RoomHub.Api.Domain.Bookings;

namespace RoomHub.Api.Repositories;

// Still in-memory (level B). Replaced by EF Core + SQLite in level C.
public class BookingRepository
{
    private readonly List<Booking> _bookings = new();
    private readonly object _lock = new();

    public void Add(Booking booking)
    {
        lock (_lock)
        {
            _bookings.Add(booking);
        }
    }

    public Booking? Get(Guid id)
    {
        lock (_lock)
        {
            return _bookings.FirstOrDefault(b => b.Id == id);
        }
    }

    // R7: is there an ACTIVE booking of this room that overlaps the interval?
    public bool HasActiveOverlap(Guid roomId, DateRange period)
    {
        lock (_lock)
        {
            return _bookings.Any(b => b.RoomId == roomId && b.ConflictsWith(period));
        }
    }
}
