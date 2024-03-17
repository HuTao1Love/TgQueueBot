using Telegram.Bot;
using Telegram.Bot.Types;
using TelegramBot.Services;

namespace TelegramBot.Commands;

public interface IChecker
{
    Task<bool> Check(ClientUpdate update, CancellationToken token);
}