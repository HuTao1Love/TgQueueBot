namespace TelegramBot.Exceptions;

public class CallbackQueryNotProvidedException : Exception
{
    private const string ExceptionText = "Callback query not provided ";

    public CallbackQueryNotProvidedException(string message)
        : base(ExceptionText + message)
    {
    }

    public CallbackQueryNotProvidedException()
        : base(ExceptionText)
    {
    }

    public CallbackQueryNotProvidedException(string message, Exception innerException)
        : base(ExceptionText + message, innerException)
    {
    }
}