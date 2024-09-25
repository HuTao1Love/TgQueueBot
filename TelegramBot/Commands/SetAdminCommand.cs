using Contracts.Repositories;
using Telegram.Bot.Types;
using TelegramBot.Rules;
using User = Models.User;

namespace TelegramBot.Commands;

[NewMessage("/admin", "/setadmin", Name = "admin", Description = "Set admin (need reply to user's message)")]
[UserIsCreatorRule("Only for Hu Tao")]
public class SetAdminCommand(IUserRepository userRepository) : ICommand
{
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