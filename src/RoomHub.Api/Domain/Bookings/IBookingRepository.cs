namespace RoomHub.Api.Domain.Bookings;

public interface IBookingRepository
{
    Task<Booking?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);

    // R7: is there an ACTIVE booking of this room overlapping the (half-open) interval?
    Task<bool> HasActiveOverlapAsync(Guid roomId, DateRange period, CancellationToken cancellationToken = default);

    // Does NOT save: IUnitOfWork.SaveChangesAsync does.
    void Add(Booking booking);
}
