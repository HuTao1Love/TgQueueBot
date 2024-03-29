﻿namespace Contracts.Repositories;

#pragma warning disable CA1711
#pragma warning disable CA2227
#pragma warning disable SK1200
public class QueueData
{
    public long Id { get; set; }

    public long TgChatId { get; set; }

    public long TgMessageId { get; set; }

    public string Name { get; set; } = null!;

    public int Size { get; set; }

    public virtual ICollection<UsersQueueData> UsersQueues { get; set; } = new List<UsersQueueData>();
}