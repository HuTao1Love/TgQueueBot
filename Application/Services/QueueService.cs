using Application.Exceptions;
using Contracts.Repositories;
using Contracts.Services;
using Models;

namespace Application.Services;

public class QueueService(IQueueRepository queueRepository, IUserQueueRepository userQueueRepository) : IQueueService
{
    public async Task<Queue> CreateQueue(long tgChatId, long tgMessageId, string name, int size)
    {
        QueueData queueInformation = await queueRepository.Create(tgChatId, tgMessageId, name, size);

        return new Queue(queueInformation.Id, queueInformation.TgChatId, queueInformation.TgMessageId, queueInformation.Name, new List<User?>(new User?[size]));
    }

    public async Task<Queue?> FindQueue(long tgChatId, long tgMessageId)
    {
        QueueData? queueData = await queueRepository.Find(tgChatId, tgMessageId);

        if (queueData is null) return null;

        var users = new List<User?>(new User?[queueData.Size]);

        IEnumerable<UsersQueueData> data = await userQueueRepository
            .FindUsersByQueueId(queueData.Id);

        data
            .Select(i => new
            {
                i.Position,
                User=new User(i.UserData.Id, i.UserData.TgId, i.UserData.Name, i.UserData.IsAdmin),
            })
            .ToList()
            .ForEach(i => users[i.Position] = i.User);

        return new Queue(queueData.Id, queueData.TgChatId, queueData.TgMessageId, queueData.Name, users);
    }

    public async Task<QueueUpdateResult> UserAction(long tgChatId, long tgMessageId, User user, int position)
    {
        Queue queue = await FindQueue(tgChatId, tgMessageId) ?? throw new QueueNotFoundException();

        int? positionOfUser = queue.Users
            .Select((u, index) => (u?.Equals(user) ?? false) ? (int?)index : null)
            .SingleOrDefault(i => i is not null, null);

        if (positionOfUser is not null)
        {
            if (positionOfUser != position)
            {
                return new QueueUpdateResult.AlreadyInQueueResult(queue);
            }

            queue.Users[position] = null;
            await userQueueRepository.RemoveUser(queue.Id, user, false);

            return new QueueUpdateResult.SuccessfulQuitResult(queue);
        }

        if (queue.Users[position] is null)
        {
            return await SetUserToPosition(queue, user, position);
        }

        var availableIndexes = queue.Users
            .Select((u, i) => u is null ? (int?)i : null)
            .Where(i => i is not null)
            .ToList();

        // first try to find last available place before user, then try to find first available place after user
        int? availableBefore = availableIndexes.LastOrDefault(i => i < position, null);

        if (availableBefore.HasValue)
        {
            return await SetUserToPosition(queue, user, availableBefore.Value);
        }

        int? availableAfter = availableIndexes.FirstOrDefault(i => i > position, null);

        return availableAfter.HasValue
            ? await SetUserToPosition(queue, user, availableAfter.Value)
            : new QueueUpdateResult.QueueIsFullResult(queue);
    }

    public async Task<Queue> ResetQueue(Queue queue)
    {
        ArgumentNullException.ThrowIfNull(queue);
        queue.Users = new List<User?>(new User?[queue.Size]);
        await userQueueRepository.RemoveUsersByQueueId(queue.Id);
        return queue;
    }

    public async Task<Queue?> DeleteQueue(long tgChatId, long tgMessageId)
    {
        Queue? queue = await FindQueue(tgChatId, tgMessageId);
        if (queue is null) return null;

        await userQueueRepository.RemoveUsersByQueueId(queue.Id);
        await queueRepository.Remove(tgChatId, tgMessageId);

        return queue;
    }

    public async Task<Queue?> RemoveUserFromQueue(long tgChatId, long tgMessageId, long userId)
    {
        Queue? queue = await FindQueue(tgChatId, tgMessageId);
        if (queue is null) return null;

        User? user = queue.Users.FirstOrDefault(u => u is not null && u.TgId == userId);
        if (user is null) return null;

        queue.Users.Remove(user);
        queue.Users.Add(null);
        await userQueueRepository.RemoveUser(queue.Id, user, true);
        return queue;
    }

    private async Task<QueueUpdateResult> SetUserToPosition(Queue queue, User user, int position)
    {
        queue.Users[position] = user;
        await userQueueRepository.AddUser(queue.Id, user, position);
        return new QueueUpdateResult.SuccessfulEnterResult(queue, position + 1);
    }
}