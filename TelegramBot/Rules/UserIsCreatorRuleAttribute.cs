using Contracts;
using Microsoft.Extensions.DependencyInjection;
using Telegram.Bot;

namespace TelegramBot.Rules;

public sealed class UserIsCreatorRuleAttribute(string? answerIfNotCreator = null) : RuleAttribute
{
    private BotConfiguration _configuration = null!;
    public override void Initialize(IServiceProvider provider)
    {
        _configuration = provider.GetRequiredService<BotConfiguration>();
    }

    public override async Task<bool> Check(ClientUpdate update, CancellationToken token)
    {
        ArgumentNullException.ThrowIfNull(update);

        long? tgId = update.Message?.From?.Id;
        long? chatId = update.Message?.Chat.Id;

        if (tgId == _configuration.BotCreator)
        {
            return true;
        }

        if (answerIfNotCreator is not null && chatId is not null)
        {
            await update.TelegramBotClient.SendTextMessageAsync(
                chatId,
                answerIfNotCreator,
                cancellationToken: token);
        }

        return false;
    }
}