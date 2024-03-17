using Telegram.Bot;
using Telegram.Bot.Types;

namespace TelegramBot.Commands;

public abstract class CommandBase
{
    protected abstract IEnumerable<IChecker> Checkers { get; }

    public async Task<bool> Check(ClientUpdate update, CancellationToken token)
    {
        ArgumentNullException.ThrowIfNull(update);

        Console.WriteLine(GetType());
        foreach (IChecker checker in Checkers)
        {
            bool passed = await checker.Check(update, token);
            if (!passed) return false;
        }

        return true;
    }

    public abstract Task Execute(ClientUpdate update, CancellationToken token);
}