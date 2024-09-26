namespace Contracts.Repositories;

public class UsersQueueData
{
    public long Id { get; set; }

    public long UserId { get; set; }

    public long QueueId { get; set; }

    public int Position { get; set; }

    public virtual QueueData QueueData { get; set; } = null!;

    public virtual UserData UserData { get; set; } = null!;
}