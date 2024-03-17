using Contracts;
using Contracts.Repositories;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using TelegramBot.Commands.Checkers;
using User = Models.User;

namespace TelegramBot.Commands.Commands;

public class MyIdCommand(BotConfiguration configuration, IUserRepository userRepository) : CommandBase
{
    protected override IEnumerable<IChecker> Checkers { get; } = new[]
    {
        new CommandChecker(configuration.BotPrefix, "myid", "me"),
    };

    public override async Task Execute(ClientUpdate update, CancellationToken token)
    {
        ArgumentNullException.ThrowIfNull(update);

        long? tgId = update.Message?.From?.Id;
        long? chatId = update.Message?.Chat?.Id;
        string? username = update.Message?.From?.Username;

        if (tgId is null || chatId is null || username is null) return;

        string text = $"Your id: ***`{tgId}`***\nThis chat id: ***`{chatId}`***";

        User fromUser = await userRepository.FindOrCreate(tgId.Value, username);

        if (fromUser.IsAdmin)
        {
            text += "\nYou is admin";
        }

        await update.AnswerText(text, ParseMode.MarkdownV2);
    }
}