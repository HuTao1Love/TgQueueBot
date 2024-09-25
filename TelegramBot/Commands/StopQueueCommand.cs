using Contracts.Services;
using Models;
using Telegram.Bot.Types;
using TelegramBot.Rules;
using TelegramBot.Services;

namespace TelegramBot.Commands;

[CallbackData(KeyboardButton.StopKeyboardButton.StopCallback)]
[UserIsAdminRule("You are not admin")]
public class StopQueueCommand(IQueueService service) : ICommand
{
    public async Task Execute(ClientUpdate update, CancellationToken token)
    {
        ArgumentNullException.ThrowIfNull(update);

        Message? message = update.CallbackQuery?.Message;
        if (message is null) return;

        Queue? queue = await service.DeleteQueue(message.Chat.Id, message.MessageId);

        await message.SafeUnpinMessageAsync(update.TelegramBotClient, token);
        await update.TelegramBotClient.EditTextAsync(
            message.Chat.Id,
            message.MessageId,
            "Stopped.\n" + queue,
            null,
            token);
    }
}