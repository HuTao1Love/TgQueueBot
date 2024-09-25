using Contracts.Repositories;
using Microsoft.Extensions.DependencyInjection;
using Telegram.Bot;
using TelegramBot.Exceptions;
using User = Models.User;

namespace TelegramBot.Rules;

public sealed class UserIsAdminRuleAttribute(string? answerIfNotAdmin = null) : RuleAttribute
{
    private IUserRepository _userRepository = null!;

    public override void Initialize(IServiceProvider provider)
    {
        _userRepository = provider.GetRequiredService<IUserRepository>();
    }

    public override async Task<bool> Check(ClientUpdate update, CancellationToken token)
    {
        ArgumentNullException.ThrowIfNull(update);

        long? tgId;
        string? username;

        if (update.CallbackQuery is not null)
        {
            tgId = update.CallbackQuery.From.Id;
            username = update.CallbackQuery.From.Username;
        }
        else if (update.Message?.From is not null)
        {
            tgId = update.Message.From.Id;
            username = update.Message.From.Username;
        }
        else
        {
            tgId = null;
            username = null;
        }

        long? chatId = update.Message?.Chat?.Id;
        if (tgId is null || username is null) return false;

        User user = await _userRepository.FindOrCreate(tgId.Value, username);
        if (user.IsAdmin) return true;

        if (answerIfNotAdmin is null || chatId is null) return false;
        try
        {
            await update.AnswerCallbackQuery(answerIfNotAdmin);
        }
        catch (CallbackQueryNotProvidedException)
        {
            await update.TelegramBotClient.SendTextMessageAsync(
                chatId,
                answerIfNotAdmin,
                cancellationToken: token);
        }

        return false;
    }
}