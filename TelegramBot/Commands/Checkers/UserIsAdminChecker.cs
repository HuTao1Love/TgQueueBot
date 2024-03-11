using Contracts.Repositories;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace TelegramBot.Commands.Checkers;

public class UserIsAdminChecker(IUserRepository userRepository) : CheckerBase
{
    protected override IEnumerable<CheckerBase>? SubCheckers { get; }

    protected override async Task<bool> Check(ITelegramBotClient telegramBotClient, Update update, CancellationToken token)
    {
        long? tgId = update?.Message?.From?.Id;
        string? username = update?.Message?.From?.Username;
        if (tgId is null || username is null) return false;

        return (await userRepository.FindOrCreate(tgId.Value, username)).IsAdmin;
    }
}