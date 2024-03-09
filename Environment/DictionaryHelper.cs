using Environment.Exceptions;

namespace Environment;

public static class DictionaryHelper
{
    public static string TryGetValue(this IDictionary<string, string> envVars, string name)
    {
        ArgumentNullException.ThrowIfNull(envVars);
        if (!envVars.TryGetValue(name, out string? value))
        {
            throw new InvalidDotEnvException($"{name} is not set");
        }

        return value;
    }
}