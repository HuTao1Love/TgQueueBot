namespace Environment.Exceptions;

public class InvalidDotEnvException : IOException
{
    public InvalidDotEnvException()
        : base("Invalid dot env")
    {
    }

    public InvalidDotEnvException(string message)
        : base("Invalid dot env: " + message)
    {
    }

    public InvalidDotEnvException(string message, Exception innerException)
        : base("Invalid dot env: " + message, innerException)
    {
    }
}