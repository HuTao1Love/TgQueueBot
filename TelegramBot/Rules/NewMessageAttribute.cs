namespace TelegramBot.Rules;

public sealed class NewMessageAttribute(params string[] commands) : MainRuleAttribute
{
    public string? Name { get; set; }
    public string? Description { get; set; }

    public override Task<bool> Check(ClientUpdate update, CancellationToken token)
    {
        string? text = update.Message?.Text;

        return Task.FromResult(text is not null && commands.Any(i => text.StartsWith(i, StringComparison.InvariantCultureIgnoreCase)));
    }
}