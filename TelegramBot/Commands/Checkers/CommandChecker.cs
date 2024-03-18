namespace TelegramBot.Commands.Checkers;

public class CommandChecker(string prefix, params string[] commands) : IChecker
{
    private readonly IEnumerable<string> _commands = commands;
    public Task<bool> Check(ClientUpdate update, CancellationToken token)
    {
        string? text = update?.Message?.Text;

        return Task.FromResult(text is not null && _commands.Any(i => text.StartsWith(prefix + i, StringComparison.InvariantCultureIgnoreCase)));
    }
}