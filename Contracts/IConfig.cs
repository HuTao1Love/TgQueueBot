namespace Contracts;

public interface IConfig
{
    public string Token { get; }
    public string Host { get; }
    public int Port { get; }
    public string Username { get; }
    public string Password { get; }
    public string Database { get; }

    public int BotCreator { get; }
    public int DefaultQueueSize { get; }
    public int MaxQueueNameLength { get; }
    public int MaxQueueSize { get; }
}