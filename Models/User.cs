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

    public override bool Equals(object? obj)
    {
        return Equals(obj as User);
    }

    public override int GetHashCode()
    {
        // ReSharper disable NonReadonlyMemberInGetHashCode
        return HashCode.Combine(UserId, TgId);

        // ReSharper restore NonReadonlyMemberInGetHashCode
    }
}