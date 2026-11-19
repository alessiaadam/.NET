using Microsoft.AspNetCore.Http.HttpResults;
using RoomHub.Api.Common;
using RoomHub.Api.Common.Cqrs;
using RoomHub.Api.Domain.Bookings;

namespace RoomHub.Api.Features.Bookings.CreateBooking;

// Nullable fields (and a nullable body): binding never fails before the room lookup,
// so a missing room always gives 404 before any validation.
public sealed record CreateBookingRequest(string? BookedBy, DateTime? StartUtc, DateTime? EndUtc, int? Attendees);

public sealed record CreateBookingResponse(Guid Id, Guid RoomId, BookingStatus Status);

public static class CreateBookingEndpoint
{
    public static IEndpointRouteBuilder MapCreateBooking(this IEndpointRouteBuilder app)
    {
        app.MapPost("/api/rooms/{roomId:guid}/bookings", Handle).WithTags("Bookings");
        return app;
    }

    private static async Task<Results<Created<CreateBookingResponse>, ProblemHttpResult>> Handle(
        Guid roomId,
        CreateBookingRequest? request,
        ICommandHandler<CreateBookingCommand, Booking> handler,
        CancellationToken cancellationToken)
    {
        var command = new CreateBookingCommand(
            roomId, request?.BookedBy, request?.StartUtc, request?.EndUtc, request?.Attendees);

        var result = await handler.Handle(command, cancellationToken);
        if (result.IsFailure)
            return result.Error!.ToProblem();

        var booking = result.Value;
        return TypedResults.Created(
            $"/api/bookings/{booking.Id}",
            new CreateBookingResponse(booking.Id, booking.RoomId, booking.Status));
    }
}
