namespace Models;

// Disable warning for name of class
#pragma warning disable CA1711
public class Queue(long id, long tgChatId, long tgMessageId, string name, IEnumerable<User?> users)
#pragma warning restore CA1711
{
    private const string GreenEmoji = "\ud83d\udfe2";
    private const string RedEmoji = "\ud83d\udd34";

    public long Id { get; } = id;
    public long TgChatId { get; } = tgChatId;
    public long TgMessageId { get; } = tgMessageId;
    public string Name { get; } = name;

    public int Size
    {
        get => Users.Count;
        set
        {
            var users = new List<User?>(value);

            for (int i = 0; i < Users.Count; i++)
            {
                users[i] = Users[i];
            }

            Users = users;
        }
    }

    public IList<User?> Users { get; private set; } = users.ToList();

    private IEnumerable<KeyboardButton> Buttons => Users
        .Select((u, index) => new KeyboardButton.UserKeyboardButton(index + 1, u is null ? GreenEmoji : RedEmoji));

    public KeyboardMarkup Markup(int maxButtonsInLine)
        => new KeyboardMarkup(Buttons, maxButtonsInLine)
            .NewLine()
            .AddItems(new KeyboardButton.ResetKeyboardButton(), new KeyboardButton.StopKeyboardButton());

    public override string ToString()
        => $"{Name}:\n{string.Concat(Users.Select((u, index) => $"{index + 1}) {u?.Name ?? GreenEmoji}\n"))}";
}