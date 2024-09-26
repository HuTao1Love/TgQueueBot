using Contracts;
using Contracts.Services;
using Models;
using Telegram.Bot;
using Telegram.Bot.Types;
using TelegramBot.Rules;
using TelegramBot.Services;
using User = Models.User;

namespace TelegramBot.Commands;

[CallbackData(KeyboardButton.SkipMeKeyboardButton.SkipMeCallback)]
public class SkipMeCommand(BotConfiguration configuration, IQueueService service) : ICommand
{
    public async Task Execute(ClientUpdate update, CancellationToken token)
    {
        ArgumentNullException.ThrowIfNull(update);

        Message? message = update.CallbackQuery?.Message;
        if (message is null) return;

        Queue? queue = await service.RemoveUserFromQueue(message.Chat.Id, message.MessageId, update.CallbackQuery!.From.Id);

        if (queue is null)
        {
            await update.TelegramBotClient.AnswerCallbackQueryAsync(
                update.CallbackQuery!.Id,
                "Вы не встали в очередь!",
                true,
                cancellationToken: token);
            return;
        }

        await update.TelegramBotClient.EditTextAsync(
            message.Chat.Id,
            message.MessageId,
            queue!.ToString(),
            queue.Markup(configuration.MaxItemsPerKeyboardLine).ToTelegramKeyboardMarkup(),
            token);
    }

    private static int? UserPosition(IEnumerable<User?> users, long id)
    {
        int index = 0;

        foreach (User? user in users)
        {
            if (user?.TgId == id)
            {
                return index;
            }

            index++;
        }

        return null;
    }
}