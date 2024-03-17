namespace TelegramBot.Exceptions;

public class MessageNotProvidedException : Exception
{
    private const string ExceptionText = "Message not provided ";

    public MessageNotProvidedException(string message)
        : base(ExceptionText + message)
    {
    }

    public MessageNotProvidedException()
        : base(ExceptionText)
    {
    }

    public MessageNotProvidedException(string message, Exception innerException)
        : base(ExceptionText + message, innerException)
    {
    }
}