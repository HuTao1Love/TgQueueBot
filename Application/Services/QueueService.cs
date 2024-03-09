using Application.Exceptions;
using Contracts.Repositories;
using Contracts.Services;
using Models;

namespace Application.Services;

public class QueueService(IQueueRepository queueRepository, IUserQueueRepository userQueueRepository) : IQueueService
{
    public async Task<Queue> CreateQueue(long tgChatId, long tgMessageId, string name, int size)
    {
        QueueData queueData = await queueRepository.Create(tgChatId, tgMessageId, name, size);

        return new Queue(queueData.Id, queueData.TgChatId, queueData.TgMessageId, queueData.Name, new List<User?>(size));
    }

    public async Task<Queue?> FindQueue(long tgChatId, long tgMessageId)
    {
        QueueData? queueData = await queueRepository.Find(tgChatId, tgMessageId);

        if (queueData is null) return null;

        var users = new List<User?>(queueData.Size);

        userQueueRepository
            .FindUsersByQueueId(queueData.Id)
            .ToBlockingEnumerable(default)
            .ToList()
            .ForEach(i => users[i.Position - 1] = i.User);

        return new Queue(queueData.Id, queueData.TgChatId, queueData.TgMessageId, queueData.Name, users);
    }

    public async Task<QueueUpdateResult> UserAction(long tgChatId, long tgMessageId, User user, int position)
    {
        --position;

        Queue queue = await FindQueue(tgChatId, tgMessageId) ?? throw new QueueNotFoundException();

        int? positionOfUser = queue.Users
            .Select((u, index) => (u?.Equals(user) ?? false) ? (int?)index : null)
            .SingleOrDefault(i => i is not null, null);

        if (positionOfUser is not null)
        {
            if (positionOfUser != position)
            {
                return new QueueUpdateResult.AlreadyInQueueResult();
            }

            queue.Users[position] = null;
            await userQueueRepository.RemoveUser(queue.Id, user);

            return new QueueUpdateResult.SuccessfulQuitResult();
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
            : new QueueUpdateResult.QueueIsFullResult();
    }

    private async Task<QueueUpdateResult> SetUserToPosition(Queue queue, User user, int position)
    {
        queue.Users[position] = user;
        await userQueueRepository.AddUser(queue.Id, user, position);
        return new QueueUpdateResult.SuccessfulEnterResult(position + 1);
    }
}