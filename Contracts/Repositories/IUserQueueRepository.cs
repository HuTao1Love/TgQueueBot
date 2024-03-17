using Models;

namespace Contracts.Repositories;

public interface IUserQueueRepository
{
    Task<IEnumerable<UsersQueueData>> FindUsersByQueueId(long id);
    Task AddUser(long id, User user, int position);
    Task RemoveUser(long id, User user);
    Task RemoveUsersByQueueId(long id);
}