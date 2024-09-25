using Contracts;
using Contracts.Repositories;
using Telegram.Bot.Types;
using TelegramBot.Rules;

namespace TelegramBot.Commands;

[NewMessage("/listq", "/listqueue", Name = "listq", Description = "Watch queue list in chat")]
[UserIsAdminRule("You are not an admin")]
public class QueueListCommand(BotConfiguration configuration, IUserRepository userRepository, IQueueRepository queueRepository) : ICommand
{
    public async Task Execute(ClientUpdate update, CancellationToken token)
    {
        ArgumentNullException.ThrowIfNull(update);

        Message? message = update.Message;
        if (message is null) return;

        IEnumerable<QueueData> queues = await queueRepository.List(message.Chat.Id);

        await update.AnswerText(string.Concat(queues.Select(i => $"{i.Name} - {i.Size} persons\n")));
    }
}