using Contracts;
using Telegram.Bot;
using TelegramBot.Commands.Checkers;

namespace TelegramBot.Commands.Commands;

// TODO
public class ShutdownCommand(BotConfiguration configuration) : CommandBase
{
    protected override IEnumerable<IChecker> Checkers { get; } = new IChecker[]
    {
        new CommandChecker(configuration.BotPrefix, "shutdown"),
        new UserIsCreatorChecker(configuration, "Only for Hu Tao"),
    };

    public override Task Execute(ClientUpdate update, CancellationToken token)
    {
        return Task.CompletedTask;
    }
}