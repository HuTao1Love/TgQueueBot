using Contracts.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Database.Repositories;

public class QueueRepository(IDbContextFactory<PostgresContext> contextFactory) : IQueueRepository
{
    public async Task<QueueData?> Find(long tgChatId, long tgMessageId)
    {
        await using PostgresContext context = await contextFactory.CreateDbContextAsync();
        QueueData? queueData = await context.Queues.FirstOrDefaultAsync(q => q.TgChatId == tgChatId && q.TgMessageId == tgMessageId);

        return queueData;
    }

    public async Task<QueueData> Create(long tgChatId, long tgMessageId, string name, int size)
    {
        await using PostgresContext context = await contextFactory.CreateDbContextAsync();
        var data = new QueueData
        {
            TgChatId = tgChatId,
            TgMessageId = tgMessageId,
            Name = name,
            Size = size,
        };

        await context.Queues.AddAsync(data);
        await context.SaveChangesAsync();

        return data;
    }

    public async Task<IEnumerable<QueueData>> List(long tgChatId)
    {
        await using PostgresContext context = await contextFactory.CreateDbContextAsync();
        return await context.Queues.Where(i => i.TgChatId == tgChatId).ToListAsync();
    }

    public async Task Remove(long tgChatId, long tgMessageId)
    {
        await using PostgresContext context = await contextFactory.CreateDbContextAsync();
        QueueData? queueData = await context.Queues.FirstOrDefaultAsync(q => q.TgChatId == tgChatId && q.TgMessageId == tgMessageId);

        if (queueData is null) return;

        context.Queues.Remove(queueData);
        await context.SaveChangesAsync();
    }
}