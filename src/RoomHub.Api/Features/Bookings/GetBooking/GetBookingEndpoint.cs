using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using RoomHub.Api.Common;
using RoomHub.Api.Domain.Bookings;
using RoomHub.Api.Infrastructure.Persistence;

namespace RoomHub.Api.Features.Bookings.GetBooking;

public sealed record GetBookingResponse(
    Guid Id,
    Guid RoomId,
    string BookedBy,
    DateTime StartUtc,
    DateTime EndUtc,
    int Attendees,
    BookingStatus Status);

public static class GetBookingEndpoint
{
    public static RouteGroupBuilder MapGetBooking(this RouteGroupBuilder group)
    {
        group.MapGet("/{id:guid}", Handle);
        return group;
    }

    // Query (read-only): no command handler, no change tracking.
    private static async Task<Results<Ok<GetBookingResponse>, ProblemHttpResult>> Handle(
        Guid id, RoomHubDbContext db, CancellationToken cancellationToken)
    {
        var b = await db.Bookings
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);

        if (b is null)
            return BookingErrors.NotFound.ToProblem();

        return TypedResults.Ok(new GetBookingResponse(
            b.Id, b.RoomId, b.BookedBy, b.Period.StartUtc, b.Period.EndUtc, b.Attendees, b.Status));
    }
}
