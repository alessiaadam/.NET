using RoomHub.Api.Models;
using RoomHub.Api.Repositories;

namespace RoomHub.Api.Services;

public class BookingService(RoomRepository rooms, BookingRepository bookings)
{
    public Booking Create(Guid roomId, string? bookedBy, DateTime? startUtc, DateTime? endUtc, int? attendees)
    {
        // Contract: 404 for a missing room, before any validation.
        var room = rooms.Get(roomId);
        if (room is null)
            throw new BusinessException("RoomNotFound", "The room does not exist.", 404);

        // R2
        if (string.IsNullOrWhiteSpace(bookedBy))
            throw new BusinessException("InvalidBookedBy", "BookedBy is required.", 400);

        if (startUtc is null || endUtc is null)
            throw new BusinessException("InvalidRange", "StartUtc and EndUtc are required.", 400);

        var start = ToUtc(startUtc.Value);
        var end = ToUtc(endUtc.Value);

        // R3: strictly after.
        if (end <= start)
            throw new BusinessException("InvalidRange", "The end must be after the start.", 400);

        // R4
        if (attendees is null or <= 0)
            throw new BusinessException("InvalidAttendees", "Attendees must be positive.", 400);

        if (attendees > room.Capacity)
            throw new BusinessException("AttendeesExceedCapacity", "The room does not have enough seats.", 400);

        // R7: only ACTIVE bookings block, and half-open intervals [start, end):
        // touching intervals (end1 == start2) do not overlap.
        foreach (var existing in bookings.ListForRoom(roomId))
        {
            if (existing.Status == BookingStatus.Active
                && existing.StartUtc < end
                && start < existing.EndUtc)
                throw new BusinessException("RoomOccupied", "The room is occupied in this interval.", 409);
        }

        var booking = new Booking
        {
            Id = Guid.NewGuid(),
            RoomId = roomId,
            BookedBy = bookedBy.Trim(),
            StartUtc = start,
            EndUtc = end,
            Attendees = attendees.Value,
            Status = BookingStatus.Active
        };

        bookings.Add(booking);
        return booking;
    }

    public Booking Get(Guid id)
    {
        var booking = bookings.Get(id);
        if (booking is null)
            throw new BusinessException("BookingNotFound", "The booking does not exist.", 404);

        return booking;
    }

    public void Cancel(Guid id)
    {
        var booking = Get(id);

        // Contract: cancelling an already cancelled booking is a conflict.
        if (booking.Status == BookingStatus.Cancelled)
            throw new BusinessException("BookingAlreadyCancelled", "The booking is already cancelled.", 409);

        booking.Status = BookingStatus.Cancelled;
    }

    // Contract: dates are UTC. "+02:00" offsets become UTC; dates without an offset are treated as UTC.
    private static DateTime ToUtc(DateTime value) => value.Kind switch
    {
        DateTimeKind.Utc => value,
        DateTimeKind.Local => value.ToUniversalTime(),
        _ => DateTime.SpecifyKind(value, DateTimeKind.Utc)
    };
}
