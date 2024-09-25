using Contracts.Repositories;
using Telegram.Bot.Types.Enums;
using TelegramBot.Rules;
using User = Models.User;

namespace TelegramBot.Commands;

[NewMessage("/myid", "/me", Name = "me", Description = "Watch my id and chat id")]
public class MyIdCommand(IUserRepository userRepository) : ICommand
{
    public async Task Execute(ClientUpdate update, CancellationToken token)
    {
        ArgumentNullException.ThrowIfNull(update);

        long? tgId = update.Message?.From?.Id;
        long? chatId = update.Message?.Chat.Id;
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