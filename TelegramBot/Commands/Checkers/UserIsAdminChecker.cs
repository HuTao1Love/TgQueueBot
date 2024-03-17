using Contracts.Repositories;
using Telegram.Bot;
using Telegram.Bot.Types;
using TelegramBot.Exceptions;
using User = Models.User;

namespace TelegramBot.Commands.Checkers;

public class UserIsAdminChecker(IUserRepository userRepository, string? answerIfNotAdmin = null) : IChecker
{
    public async Task<bool> Check(ClientUpdate update, CancellationToken token)
    {
        ArgumentNullException.ThrowIfNull(update);

        long? tgId;
        string? username;

        if (update.CallbackQuery is not null)
        {
            tgId = update.CallbackQuery.From.Id;
            username = update.CallbackQuery.From.Username;
        }
        else if (update.Message?.From is not null)
        {
            tgId = update.Message.From.Id;
            username = update.Message.From.Username;
        }
        else
        {
            tgId = null;
            username = null;
        }

        long? chatId = update.Message?.Chat?.Id;
        if (tgId is null || username is null) return false;

        User user = await userRepository.FindOrCreate(tgId.Value, username);
        if (user.IsAdmin) return true;

        if (answerIfNotAdmin is null || chatId is null) return false;
        try
        {
            await update.AnswerCallbackQuery(answerIfNotAdmin);
        }
        catch (CallbackQueryNotProvidedException)
        {
            await update.TelegramBotClient.SendTextMessageAsync(
                chatId,
                answerIfNotAdmin,
                cancellationToken: token);
        }

        return false;
    }
}