using RoomHub.Api.Services;

namespace RoomHub.Api.Features.Bookings.CancelBooking;

public static class CancelBookingEndpoint
{
    public static RouteGroupBuilder MapCancelBooking(this RouteGroupBuilder group)
    {
        group.MapPost("/{id:guid}/cancel", (Guid id, BookingService service) =>
        {
            service.Cancel(id);
            return Results.NoContent();
        });

        return group;
    }
}
