namespace TelegramBot.Rules;

public sealed class CallbackDataAttribute(params string[] dataStartWith) : MainRuleAttribute
{
    public override Task<bool> Check(ClientUpdate update, CancellationToken token)
    {
        string? callback = update.CallbackQuery?.Data;
        return Task.FromResult(callback is not null && dataStartWith.Any(
            i => callback.StartsWith(i, StringComparison.OrdinalIgnoreCase)));
    }
}