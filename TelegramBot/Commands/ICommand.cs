namespace TelegramBot.Commands;

public interface ICommand
{
    string? Name { get; }
    string? Description { get; }

    IEnumerable<IChecker> Checkers { get; }

    Task Execute(ClientUpdate update, CancellationToken token);
}