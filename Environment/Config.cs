using System.Globalization;
using Contracts;
using dotenv.net;
using Environment.Exceptions;

namespace Environment;

public class Config : IConfig
{
    private static readonly IEnumerable<string> EnvFilePaths = new[] { ".env" };
    private static Config? _instance;

    private Config()
    {
        DotEnv.Load(new DotEnvOptions(
            ignoreExceptions: true,
            envFilePaths: EnvFilePaths));

        IDictionary<string, string> envVars = DotEnv.Read() ?? throw new InvalidDotEnvException("Dot enc not found");

        Token = envVars.TryGetValue("API_TOKEN");
        Host = envVars.TryGetValue("POSTGRES_HOST");
        Port = Convert.ToInt32(envVars.TryGetValue("POSTGRES_PORT"), CultureInfo.InvariantCulture);
        Username = envVars.TryGetValue("POSTGRES_USERNAME");
        Password = envVars.TryGetValue("POSTGRES_PASSWORD");
        Database = envVars.TryGetValue("POSTGRES_DATABASE");
    }

    public static Config Instance => _instance ??= new Config();

    public int BotCreator => 751586125;
    public int DefaultQueueSize => 25;
    public int MaxQueueNameLength => 30;
    public int MaxQueueSize => 98;  // because tg button limit is 100; need reset & stop buttons
    public string Token { get; }
    public string Host { get; }
    public int Port { get; }
    public string Username { get; }
    public string Password { get; }
    public string Database { get; }
}