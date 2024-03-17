using Contracts;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace TelegramBot.Commands.Checkers;

public class UserIsCreatorChecker(BotConfiguration configuration, string? answerIfNotCreator = null) : IChecker
{
    public async Task<bool> Check(ClientUpdate update, CancellationToken token)
    {
        ArgumentNullException.ThrowIfNull(update);

        long? tgId = update.Message?.From?.Id;
        long? chatId = update.Message?.Chat?.Id;

        if (tgId == configuration.BotCreator)
        {
            return true;
        }

        if (answerIfNotCreator is not null && chatId is not null)
        {
            await update.TelegramBotClient.SendTextMessageAsync(
                chatId,
                answerIfNotCreator,
                cancellationToken: token);
        }

        return false;
    }
}