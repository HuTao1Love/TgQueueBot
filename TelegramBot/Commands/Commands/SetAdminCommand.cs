using Contracts;
using Contracts.Repositories;
using Telegram.Bot.Types;
using TelegramBot.Commands.Checkers;
using User = Models.User;

namespace TelegramBot.Commands.Commands;

public class SetAdminCommand(BotConfiguration configuration, IUserRepository userRepository) : ICommand
{
    public string? Name => "admin";
    public string? Description => "Set admin (need reply to user's message)";

    public IEnumerable<IChecker> Checkers { get; } = new IChecker[]
    {
        new CommandChecker(configuration.BotPrefix, "admin", "setadmin"),
        new UserIsCreatorChecker(configuration, "Only for Hu Tao"),
    };

    public async Task Execute(ClientUpdate update, CancellationToken token)
    {
        ArgumentNullException.ThrowIfNull(update);

        Message? replyToMessage = update.Message?.ReplyToMessage;

        if (replyToMessage?.From?.Username is null)
        {
            await update.AnswerText("Please, reply to user, that you wanna set admin");
            return;
        }

        User user = await userRepository.FindOrCreate(replyToMessage.From.Id, replyToMessage.From.Username);
        await userRepository.SetAdmin(user.TgId, !user.IsAdmin);

        await update.AnswerText($"@{replyToMessage.From.Username} now admin: {!user.IsAdmin}");
    }
}