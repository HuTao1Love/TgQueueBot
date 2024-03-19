using Contracts;
using Contracts.Repositories;
using Telegram.Bot.Types;
using TelegramBot.Commands.Checkers;

namespace TelegramBot.Commands.Commands;

public class QueueListCommand(BotConfiguration configuration, IUserRepository userRepository, IQueueRepository queueRepository) : ICommand
{
    public IEnumerable<IChecker> Checkers { get; } = new IChecker[]
    {
        new CommandChecker(configuration.BotPrefix, "listq", "listqueue"),
        new UserIsAdminChecker(userRepository, "You are not an admin"),
    };

    public async Task Execute(ClientUpdate update, CancellationToken token)
    {
        ArgumentNullException.ThrowIfNull(update);

        Message? message = update.Message;
        if (message is null) return;

        IEnumerable<QueueData> queues = await queueRepository.List(message.Chat.Id);

        await update.AnswerText(string.Concat(queues.Select(i => $"{i.Name} - {i.Size} persons\n")));
    }
}