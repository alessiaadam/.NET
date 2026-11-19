using RoomHub.Api.Common.Cqrs;
using RoomHub.Api.Domain.Bookings;
using RoomHub.Api.Domain.Common;

namespace RoomHub.Api.Features.Bookings.CancelBooking;

public sealed record CancelBookingCommand(Guid BookingId) : ICommand<Unit>;

// Depends only on domain interfaces (no EF Core here).
public sealed class CancelBookingHandler(IBookingRepository bookings, IUnitOfWork unitOfWork)
    : ICommandHandler<CancelBookingCommand, Unit>
{
    public async Task<Result<Unit>> Handle(CancelBookingCommand command, CancellationToken cancellationToken)
    {
        // R8: missing -> 404.
        var booking = await bookings.GetByIdAsync(command.BookingId, cancellationToken);
        if (booking is null)
            return BookingErrors.NotFound;

        // R8 lives in the domain (Active -> Cancelled, twice -> 409).
        var result = booking.Cancel();
        if (result.IsFailure)
            return result;

        // The booking is tracked by EF Core: saving persists the new Status.
        await unitOfWork.SaveChangesAsync(cancellationToken);
        return result;
    }
}
