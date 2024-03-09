using Models;

namespace Contracts.Services;

public interface IQueueService
{
    Task<Queue> CreateQueue(long tgChatId, long tgMessageId, string name, int size);
    Task<Queue?> FindQueue(long tgChatId, long tgMessageId);
    Task<QueueUpdateResult> UserAction(long tgChatId, long tgMessageId, User user, int position);
}