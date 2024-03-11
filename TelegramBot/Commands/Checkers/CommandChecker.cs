using Telegram.Bot;
using Telegram.Bot.Types;

namespace TelegramBot.Commands.Checkers;

public class CommandChecker(string prefix, params string[] commands) : CheckerBase
{
    private readonly IEnumerable<string> _commands = commands;

    protected override IEnumerable<CheckerBase>? SubCheckers { get; } = new[] { new MessageTextNotNullChecker() };

    protected override Task<bool> Check(ITelegramBotClient telegramBotClient, Update update, CancellationToken token)
    {
        string? text = update?.Message?.Text;

        return Task.FromResult(text is not null && _commands.Any(i => text.StartsWith(prefix + i, StringComparison.InvariantCultureIgnoreCase)));
    }
}