using RoomHub.Api.Features.Bookings.CancelBooking;
using RoomHub.Api.Features.Bookings.CreateBooking;
using RoomHub.Api.Features.Bookings.GetBooking;

namespace RoomHub.Api.Features.Bookings;

public static class BookingsEndpoints
{
    public static IEndpointRouteBuilder MapBookingsEndpoints(this IEndpointRouteBuilder app)
    {
        // POST /api/rooms/{roomId}/bookings belongs to the Bookings feature, but lives under the room route.
        app.MapCreateBooking();

        var group = app.MapGroup("/api/bookings").WithTags("Bookings");
        group.MapGetBooking();
        group.MapCancelBooking();

        return app;
    }
}
