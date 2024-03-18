namespace TelegramBot.Commands;

public interface IChecker
{
    Task<bool> Check(ClientUpdate update, CancellationToken token);
}