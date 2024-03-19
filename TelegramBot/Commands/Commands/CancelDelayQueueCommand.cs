using Contracts.Repositories;
using Models;
using TelegramBot.Commands.Checkers;
using TelegramBot.Services;

namespace TelegramBot.Commands.Commands;

public class CancelDelayQueueCommand(BotContext context, IUserRepository userRepository) : ICommand
{
    public string? Name => null;
    public string? Description => null;

    public IEnumerable<IChecker> Checkers { get; } = new IChecker[]
    {
        new CallbackDataChecker(KeyboardButton.CancelKeyboardButton.CancelCallback),
        new UserIsAdminChecker(userRepository, "You must be an admin to do this"),
    };

    public async Task Execute(ClientUpdate update, CancellationToken token)
    {
        ArgumentNullException.ThrowIfNull(update);
        if (update.CallbackQuery?.Message is not { } message) return;

        await message.EditTextAsync(update.TelegramBotClient, "Queue creation stopped", null, token);
        if (context.CancellationTokenDictionary.TryGetValue(
                new MessageIdentifier(message.Chat.Id, message.MessageId),
                out CancellationTokenSource? cts))
        {
            await cts.CancelAsync();
        }
    }
}