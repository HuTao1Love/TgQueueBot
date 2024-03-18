using Models;

namespace Contracts.Services;

public abstract record QueueUpdateResult
{
    private QueueUpdateResult(Queue queue, bool isSuccessful, string message)
    {
        Queue = queue;
        IsSuccessful = isSuccessful;
        Message = message;
    }

    public Queue Queue { get; }
    public bool IsSuccessful { get; }
    public string Message { get; }

    public sealed record QueueIsFullResult(Queue Queue)
        : QueueUpdateResult(Queue, false, "Очередь полная!");

    public sealed record SuccessfulQuitResult(Queue Queue)
        : QueueUpdateResult(Queue, true, "Успешно отписался{ась}!");

    public sealed record SuccessfulEnterResult(Queue Queue, int Position)
        : QueueUpdateResult(Queue, true, $"Успешная запись на место {Position}!")
    {
        public int Position { get; init; } = Position;
    }

    public sealed record AlreadyInQueueResult(Queue Queue)
        : QueueUpdateResult(Queue, false, "Вы уже в очереди");
}