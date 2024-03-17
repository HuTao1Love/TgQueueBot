using System.Globalization;
using Contracts;
using Contracts.Repositories;
using Contracts.Services;
using Models;
using Telegram.Bot.Types;
using TelegramBot.Commands.Checkers;
using TelegramBot.Exceptions;
using TelegramBot.Services;
using User = Models.User;

namespace TelegramBot.Commands.Commands;

public class UpdatePositionCommand(BotConfiguration configuration, IQueueService service, IUserRepository userRepository) : CommandBase
{
    protected override IEnumerable<IChecker> Checkers { get; } = new IChecker[]
    {
        new CallbackDataChecker(KeyboardButton.UserKeyboardButton.CallbackPrefix),
    };

    public override async Task Execute(ClientUpdate update, CancellationToken token)
    {
        ArgumentNullException.ThrowIfNull(update);

        CallbackQuery? callbackQuery = update.CallbackQuery;
        Message? message = callbackQuery?.Message;
        if (callbackQuery is null || message is null) return;

        Queue? queue = await service.FindQueue(message.Chat.Id, message.MessageId);
        if (queue is null) return;

        int position = Convert.ToInt32(
            callbackQuery.Data?.Replace(
                KeyboardButton.UserKeyboardButton.CallbackPrefix,
                string.Empty,
                StringComparison.InvariantCultureIgnoreCase),
            CultureInfo.InvariantCulture);

        Telegram.Bot.Types.User from = update.CallbackQuery?.From ?? throw new CallbackQueryNotProvidedException();
        User user = await userRepository.FindOrCreate(from.Id, from.Username ?? string.Empty);

        QueueUpdateResult result = await service.UserAction(message.Chat.Id, message.MessageId, user, position);

        await update.AnswerCallbackQuery(result.Message);

        if (result.IsSuccessful)
        {
            await Task.Delay(100, token);
            await update.TelegramBotClient.EditTextAsync(message.Chat.Id, message.MessageId, result.Queue.ToString(), result.Queue.Markup(configuration.MaxItemsPerKeyboardLine).ToTelegramKeyboardMarkup(), token);
        }
    }
}