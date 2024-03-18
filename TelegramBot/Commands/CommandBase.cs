namespace TelegramBot.Commands;

public abstract class CommandBase
{
    protected abstract IEnumerable<IChecker> Checkers { get; }

    public async Task<bool> Check(ClientUpdate update, CancellationToken token)
    {
        ArgumentNullException.ThrowIfNull(update);

        foreach (IChecker checker in Checkers)
        {
            if (!await checker.Check(update, token)) return false;
        }

        return true;
    }

    public abstract Task Execute(ClientUpdate update, CancellationToken token);
}