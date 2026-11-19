using Microsoft.AspNetCore.Http.HttpResults;
using RoomHub.Api.Common;
using RoomHub.Api.Domain.Bookings;
using RoomHub.Api.Repositories;

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

    // Query (read-only): no command handler needed.
    private static Results<Ok<GetBookingResponse>, ProblemHttpResult> Handle(Guid id, BookingRepository bookings)
    {
        var b = bookings.Get(id);
        if (b is null)
            return BookingErrors.NotFound.ToProblem();

        return TypedResults.Ok(new GetBookingResponse(
            b.Id, b.RoomId, b.BookedBy, b.Period.StartUtc, b.Period.EndUtc, b.Attendees, b.Status));
    }
}
