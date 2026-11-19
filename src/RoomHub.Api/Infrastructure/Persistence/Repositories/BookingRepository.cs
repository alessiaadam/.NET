using Microsoft.EntityFrameworkCore;
using RoomHub.Api.Domain.Bookings;

namespace RoomHub.Api.Infrastructure.Persistence.Repositories;

public sealed class BookingRepository(RoomHubDbContext db) : IBookingRepository
{
    public async Task<Booking?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default) =>
        await db.Bookings.FirstOrDefaultAsync(b => b.Id == id, cancellationToken);

    // R7: only ACTIVE bookings, half-open intervals (strict "<"): touching intervals do not overlap,
    // and a cancelled booking frees its interval (R8).
    public Task<bool> HasActiveOverlapAsync(
        Guid roomId, DateRange period, CancellationToken cancellationToken = default)
    {
        var start = period.StartUtc;
        var end = period.EndUtc;

        return db.Bookings.AnyAsync(
            b => b.RoomId == roomId
                 && b.Status == BookingStatus.Active
                 && b.Period.StartUtc < end
                 && start < b.Period.EndUtc,
            cancellationToken);
    }

    // No SaveChangesAsync here: the Unit of Work commits.
    public void Add(Booking booking) => db.Bookings.Add(booking);
}
