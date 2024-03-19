namespace TelegramBot.Commands;

public interface ICommand
{
    IEnumerable<IChecker> Checkers { get; }

    Task Execute(ClientUpdate update, CancellationToken token);
}