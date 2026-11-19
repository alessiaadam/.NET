namespace RoomHub.Api.Domain.Common;

// Commits all changes made through the repositories, in one transaction.
public interface IUnitOfWork
{
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}
