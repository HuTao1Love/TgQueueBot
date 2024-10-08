using Contracts.Repositories;
using Microsoft.EntityFrameworkCore;
using Models;

namespace Database.Repositories;

public class UserQueueRepository(IDbContextFactory<PostgresContext> contextFactory) : IUserQueueRepository
{
    public async Task<IEnumerable<UsersQueueData>> FindUsersByQueueId(long id)
    {
        await using PostgresContext context = await contextFactory.CreateDbContextAsync();
        return await context.UsersQueues.Where(i => i.QueueId == id)
            .Include(usersQueueData => usersQueueData.UserData).ToListAsync();
    }

    public async Task AddUser(long id, User user, int position)
    {
        ArgumentNullException.ThrowIfNull(user);
        await using PostgresContext context = await contextFactory.CreateDbContextAsync();
        await context.UsersQueues.AddAsync(new UsersQueueData()
        {
            UserId = user.UserId,
            QueueId = id,
            Position = position,
        });
        await context.SaveChangesAsync();
    }

    public async Task RemoveUser(long id, User user, bool deletePosition)
    {
        ArgumentNullException.ThrowIfNull(user);
        await using PostgresContext context = await contextFactory.CreateDbContextAsync();
        UsersQueueData? usersQueueData = await context.UsersQueues.FirstOrDefaultAsync(i => i.QueueId == id && i.UserId == user.UserId);

        if (usersQueueData is null) return;

        if (deletePosition)
        {
            int position = usersQueueData.Position;

            await context.UsersQueues
                .Where(uq => uq.QueueId == id && position < uq.Position)
                .ExecuteUpdateAsync(uq => uq.SetProperty(
                    i => i.Position,
                    i => int.Max(i.Position - 1, 0)));
        }

        context.UsersQueues.Remove(usersQueueData);

        await context.SaveChangesAsync();
    }

    public async Task RemoveUsersByQueueId(long id)
    {
        await using PostgresContext context = await contextFactory.CreateDbContextAsync();

        context.UsersQueues.RemoveRange(context.UsersQueues.Where(i => i.QueueId == id));
        await context.SaveChangesAsync();
    }
}