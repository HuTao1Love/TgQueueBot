namespace Application.Exceptions;

public class QueueNotFoundException : Exception
{
    public QueueNotFoundException(string message)
        : base($"Queue not found " + message)
    {
    }

    public QueueNotFoundException()
        : base($"Queue not found")
    {
    }

    public QueueNotFoundException(string message, Exception innerException)
        : base($"Queue not found " + message, innerException)
    {
    }
}