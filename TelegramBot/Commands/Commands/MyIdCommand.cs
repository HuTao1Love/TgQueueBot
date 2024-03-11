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
    protected override IEnumerable<CheckerBase> Checkers { get; } = new[]
    {
        new CommandChecker(configuration.BotPrefix, "myid", "me"),
    };

    protected override async Task Execute(ClientUpdate update, CancellationToken token)
    {
        ArgumentNullException.ThrowIfNull(update);

        string text = $"Your id: ***`{update.Message!.From!.Id}`***\nThis chat id: ***`{update.Message!.Chat!.Id}`***";

        User fromUser = await userRepository.FindOrCreate(update.Message!.From!.Id, update.Message!.From!.Username!);

        if (fromUser.IsAdmin)
        {
            text += "\nYou is admin";
        }

        await update.AnswerText(text, ParseMode.MarkdownV2);
    }
}