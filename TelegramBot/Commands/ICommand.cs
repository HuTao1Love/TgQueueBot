namespace TelegramBot.Commands;

public interface ICommand
{
    Task Execute(ClientUpdate update, CancellationToken token);
}