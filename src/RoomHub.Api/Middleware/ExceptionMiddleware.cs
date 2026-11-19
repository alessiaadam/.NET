using RoomHub.Api.Services;

namespace RoomHub.Api.Middleware;

public sealed class ExceptionMiddleware(RequestDelegate next)
{
    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await next(context);
        }
        catch (BusinessException ex)
        {
            await Results.Problem(
                    detail: ex.Message,
                    statusCode: ex.StatusCode,
                    title: ex.Code)
                .ExecuteAsync(context);
        }
        catch (BadHttpRequestException ex)
        {
            // Malformed JSON / unreadable body is a client error (400), not a 500.
            await Results.Problem(
                    detail: ex.Message,
                    statusCode: StatusCodes.Status400BadRequest,
                    title: "InvalidRequest")
                .ExecuteAsync(context);
        }
    }
}
