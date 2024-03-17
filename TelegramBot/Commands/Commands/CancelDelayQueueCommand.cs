using Contracts.Repositories;
using Models;
using TelegramBot.Commands.Checkers;
using TelegramBot.Services;

namespace TelegramBot.Commands.Commands;

public class CancelDelayQueueCommand(IUserRepository userRepository) : CommandBase
{
    protected override IEnumerable<IChecker> Checkers { get; } = new IChecker[]
    {
        new CallbackDataChecker(KeyboardButton.CancelKeyboardButton.CancelCallback),
        new UserIsAdminChecker(userRepository, "You must be an admin to do this"),
    };

    public override async Task Execute(ClientUpdate update, CancellationToken token)
    {
        ArgumentNullException.ThrowIfNull(update);
        if (update.Message is null) return;

        // now it's only removes button, TODO stop task.Delay in DelayCreateQueueCommand
        await update.Message.EditTextAsync(update.TelegramBotClient, "Queue creation stopped", null, token);
    }
}