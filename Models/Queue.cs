namespace Models;

#pragma warning disable CA1711
#pragma warning disable CA2227
public class Queue(long id, long tgChatId, long tgMessageId, string name, IEnumerable<User?> users)
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
            var users = new List<User?>(new User?[value]);

            for (int i = 0; i < Users.Count; i++)
            {
                users[i] = Users[i];
            }

            Users = users;
        }
    }

    public IList<User?> Users { get; set; } = users.ToList();

    private IEnumerable<KeyboardButton> Buttons => Users
        .Select((u, index) => new KeyboardButton.UserKeyboardButton(index, u is null ? GreenEmoji : RedEmoji));

    public KeyboardMarkup Markup(int maxButtonsInLine)
        => new KeyboardMarkup(Buttons, maxButtonsInLine)
            .NewLine()
            .AddItems(new KeyboardButton.SkipMeKeyboardButton(), new KeyboardButton.ResetKeyboardButton(), new KeyboardButton.StopKeyboardButton());

    public override string ToString()
        => $"{Name}:\n{string.Concat(Users.Select((u, index) => $"{index + 1}) @{u?.Name ?? GreenEmoji}\n"))}";
}