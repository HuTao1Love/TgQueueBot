using System.Collections.Concurrent;

namespace TelegramBot.Services;

public class BotContext
{
    public CancellationTokenSource UpdateReceivingTokenSource { get; } = new CancellationTokenSource();
    public ConcurrentDictionary<MessageIdentifier, CancellationTokenSource> CancellationTokenDictionary { get; } = new();
}