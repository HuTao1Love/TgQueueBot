namespace Models;

public class User(long userId, long tgId, string name, bool isAdmin)
    : IEquatable<User>
{
    public long UserId { get; set; } = userId;
    public long TgId { get; set; } = tgId;
    public string Name { get; set; } = name ?? throw new ArgumentNullException(nameof(name));
    public bool IsAdmin { get; set; } = isAdmin;

    public bool Equals(User? other)
    {
        if (other is null) return false;

        return UserId == other.UserId && TgId == other.TgId;
    }

#pragma warning disable CS0659 // Type overrides Object.Equals(object o) but does not override Object.GetHashCode()
    public override bool Equals(object? obj)
#pragma warning restore CS0659 // Type overrides Object.Equals(object o) but does not override Object.GetHashCode()
    {
        return Equals(obj as User);
    }
}