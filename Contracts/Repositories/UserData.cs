namespace Contracts.Repositories;

#pragma warning disable CA2227
#pragma warning disable SK1200
public class UserData
{
    public long Id { get; set; }

    public long TgId { get; set; }

    public string Name { get; set; } = null!;

    public bool IsAdmin { get; set; }

    public virtual ICollection<UsersQueueData> UsersQueues { get; set; } = new List<UsersQueueData>();
}