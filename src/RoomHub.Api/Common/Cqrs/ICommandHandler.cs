using RoomHub.Api.Domain.Common;

namespace RoomHub.Api.Common.Cqrs;

// A command changes state and returns a Result<TResult>.
public interface ICommand<TResult>;

public interface ICommandHandler<in TCommand, TResult>
    where TCommand : ICommand<TResult>
{
    Task<Result<TResult>> Handle(TCommand command, CancellationToken cancellationToken);
}
