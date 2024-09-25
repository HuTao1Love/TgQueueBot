using Contracts.Repositories;
using Models;
using TelegramBot.Rules;
using TelegramBot.Services;

namespace TelegramBot.Commands;

[CallbackData(KeyboardButton.CancelKeyboardButton.CancelCallback)]
[UserIsAdminRule("You must be an admin to do this")]
public class CancelDelayQueueCommand(BotContext context, IUserRepository userRepository) : ICommand
{
    public async Task Execute(ClientUpdate update, CancellationToken token)
    {
        ArgumentNullException.ThrowIfNull(update);
        if (update.CallbackQuery?.Message is not { } message) return;

        await message.EditTextAsync(update.TelegramBotClient, "Queue creation stopped", null, token);
        await message.SafeUnpinMessageAsync(update.TelegramBotClient, token);
        if (context.CancellationTokenDictionary.TryGetValue(
                message.ToMessageIdentifier(),
                out CancellationTokenSource? cts))
        {
            await cts.CancelAsync();
        }
    }
}