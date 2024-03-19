using Contracts.Repositories;
using Contracts.Services;
using Models;
using Telegram.Bot.Types;
using TelegramBot.Commands.Checkers;
using TelegramBot.Services;

namespace TelegramBot.Commands.Commands;

public class StopQueueCommand(IQueueService service, IUserRepository userRepository) : ICommand
{
    public string? Name => null;
    public string? Description => null;

    public IEnumerable<IChecker> Checkers { get; } = new IChecker[]
    {
        new CallbackDataChecker(KeyboardButton.StopKeyboardButton.StopCallback),
        new UserIsAdminChecker(userRepository, "You are not admin"),
    };

    public async Task Execute(ClientUpdate update, CancellationToken token)
    {
        ArgumentNullException.ThrowIfNull(update);

        Message? message = update.CallbackQuery?.Message;
        if (message is null) return;

        Queue? queue = await service.DeleteQueue(message.Chat.Id, message.MessageId);

        await update.TelegramBotClient.EditTextAsync(
            message.Chat.Id,
            message.MessageId,
            "Stopped.\n" + queue ?? string.Empty,
            null,
            token);
    }
}