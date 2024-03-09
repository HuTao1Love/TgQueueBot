namespace Contracts.Services;

public abstract record QueueUpdateResult
{
    private QueueUpdateResult(bool isSuccessful, string message)
    {
        IsSuccessful = isSuccessful;
        Message = message;
    }

    public bool IsSuccessful { get; }
    public string Message { get; }

    public sealed record QueueIsFullResult()
        : QueueUpdateResult(false, "Очередь полная!");

    public sealed record SuccessfulQuitResult()
        : QueueUpdateResult(true, "Успешно отписался{ась}!");

    public sealed record SuccessfulEnterResult(int Position)
        : QueueUpdateResult(true, $"Успешная запись на место {Position}!")
    {
        public int Position { get; init; } = Position;
    }

    public sealed record AlreadyInQueueResult()
        : QueueUpdateResult(false, "Вы уже в очереди");
}