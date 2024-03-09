namespace Contracts.Repositories;

public record QueueData(long Id, long TgChatId, long TgMessageId, string Name, int Size);