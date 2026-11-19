using System.Text.Json.Serialization;
using Microsoft.EntityFrameworkCore;
using RoomHub.Api.Common.Cqrs;
using RoomHub.Api.Domain.Bookings;
using RoomHub.Api.Domain.Common;
using RoomHub.Api.Domain.Rooms;
using RoomHub.Api.Features.Bookings;
using RoomHub.Api.Features.Rooms;
using RoomHub.Api.Infrastructure.Persistence;
using RoomHub.Api.Infrastructure.Persistence.Repositories;

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

// Persistence: EF Core + SQLite. The file sits next to the app (content root), so data survives a restart.
var connectionString = builder.Configuration.GetConnectionString("RoomHub")
    ?? $"Data Source={Path.Combine(builder.Environment.ContentRootPath, "roomhub.db")}";

builder.Services.AddDbContext<RoomHubDbContext>(options => options.UseSqlite(connectionString));
builder.Services.AddScoped<IUnitOfWork>(sp => sp.GetRequiredService<RoomHubDbContext>());
builder.Services.AddScoped<IRoomRepository, RoomRepository>();
builder.Services.AddScoped<IBookingRepository, BookingRepository>();

// All ICommandHandler<,> implementations, registered as Scoped.
builder.Services.AddHandlers(typeof(Program).Assembly);

var app = builder.Build();

await app.InitializeDatabaseAsync();

app.UseExceptionHandler();
app.UseStatusCodePages();

app.MapGet("/", () => "RoomHub API is running.");
app.MapRoomsEndpoints();
app.MapBookingsEndpoints();

app.Run();
