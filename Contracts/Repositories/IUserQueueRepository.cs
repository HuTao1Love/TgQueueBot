using Models;

namespace Contracts.Repositories;

public interface IUserQueueRepository
{
    IAsyncEnumerable<UserInformation> FindUsersByQueueId(long id);
    Task AddUser(long id, User user, int position);
    Task RemoveUser(long id, User user);
    Task UpdateUserPosition(long id, User user, int newPosition);
}