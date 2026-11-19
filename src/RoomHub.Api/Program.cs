using System.Text.Json.Serialization;
using RoomHub.Api.Common.Cqrs;
using RoomHub.Api.Features.Bookings;
using RoomHub.Api.Features.Rooms;
using RoomHub.Api.Repositories;

var builder = WebApplication.CreateBuilder(args);

// Contract: enums appear in JSON as text ("Active" / "Cancelled").
builder.Services.ConfigureHttpJsonOptions(options =>
    options.SerializerOptions.Converters.Add(new JsonStringEnumConverter()));

// Framework errors (bad JSON, unhandled exceptions) are also returned as application/problem+json.
builder.Services.AddProblemDetails();

// Malformed JSON gives an empty 400 (turned into ProblemDetails by UseStatusCodePages),
// instead of throwing BadHttpRequestException (a 500) in Development.
builder.Services.Configure<RouteHandlerOptions>(options => options.ThrowOnBadRequest = false);

// "Now" comes only from TimeProvider (R5).
builder.Services.AddSingleton(TimeProvider.System);

// Still in-memory (level B). Replaced by EF Core + SQLite in level C.
builder.Services.AddSingleton<RoomRepository>();
builder.Services.AddSingleton<BookingRepository>();

// All ICommandHandler<,> implementations, registered as Scoped.
builder.Services.AddHandlers(typeof(Program).Assembly);

var app = builder.Build();

app.UseExceptionHandler();
app.UseStatusCodePages();

app.MapGet("/", () => "RoomHub API is running.");
app.MapRoomsEndpoints();
app.MapBookingsEndpoints();

app.Run();
