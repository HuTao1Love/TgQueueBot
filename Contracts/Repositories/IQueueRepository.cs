using Models;

namespace Contracts.Repositories;

public interface IQueueRepository
{
    Task<QueueData?> Find(long tgChatId, long tgMessageId);
    Task<QueueData> Create(long tgChatId, long tgMessageId, string name, int size);
    Task Remove(long tgChatId, long tgMessageId);
}