using RoomHub.Api.Models;
using RoomHub.Api.Services;

namespace RoomHub.Api.Features.Bookings.CreateBooking;

// Nullable fields (and a nullable body): binding never fails before the room lookup,
// so a missing room always gives 404 before any validation.
public sealed record CreateBookingRequest(string? BookedBy, DateTime? StartUtc, DateTime? EndUtc, int? Attendees);

public sealed record CreateBookingResponse(Guid Id, Guid RoomId, BookingStatus Status);

public static class CreateBookingEndpoint
{
    public static IEndpointRouteBuilder MapCreateBooking(this IEndpointRouteBuilder app)
    {
        app.MapPost("/api/rooms/{roomId:guid}/bookings",
                (Guid roomId, CreateBookingRequest? request, BookingService service) =>
                {
                    var booking = service.Create(
                        roomId, request?.BookedBy, request?.StartUtc, request?.EndUtc, request?.Attendees);

                    return Results.Created(
                        $"/api/bookings/{booking.Id}",
                        new CreateBookingResponse(booking.Id, booking.RoomId, booking.Status));
                })
            .WithTags("Bookings");

        return app;
    }
}
