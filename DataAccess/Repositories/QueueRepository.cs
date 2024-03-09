using Contracts.Repositories;
using Itmo.Dev.Platform.Postgres.Connection;
using Npgsql;

namespace DataAccess.Repositories;

public class QueueRepository(IPostgresConnectionProvider provider) : IQueueRepository
{
    public async Task<QueueData?> Find(long tgChatId, long tgMessageId)
    {
        NpgsqlConnection connection = await provider.GetConnectionAsync(default);
        NpgsqlCommand command = connection.CreateCommand();

        command.CommandText = """
                              select id, tgChatId, tgMessageId, name, size
                              from queues
                              where tgChatId = @tgChatId and tgMessageId = @tgMessageId;
                              """;

        command.Parameters.AddWithValue("tgChatId", tgChatId);
        command.Parameters.AddWithValue("tgMessageId", tgMessageId);

        await using NpgsqlDataReader reader = await command.ExecuteReaderAsync();

        if (!await reader.ReadAsync())
        {
            return null;
        }

        return new QueueData(
            reader.GetInt64(0),
            tgChatId,
            tgMessageId,
            reader.GetString(3),
            reader.GetInt32(4));
    }

    public async Task<QueueData> Create(long tgChatId, long tgMessageId, string name, int size)
    {
        NpgsqlConnection connection = await provider.GetConnectionAsync(default);
        NpgsqlCommand command = connection.CreateCommand();

        command.CommandText = """
                              insert into queues (tgchatid, tgmessageid, name, size)
                              values (@tgChatId, @tgMessageId, @name, @size)
                              returning id;
                              """;

        command.Parameters.AddWithValue("tgChatId", tgChatId);
        command.Parameters.AddWithValue("tgMessageId", tgMessageId);
        command.Parameters.AddWithValue("name", name);
        command.Parameters.AddWithValue("size", size);

        await using NpgsqlDataReader reader = await command.ExecuteReaderAsync();
        await reader.ReadAsync();

        return new QueueData(reader.GetInt64(0), tgChatId, tgMessageId, name, size);
    }

    public async Task Remove(long tgChatId, long tgMessageId)
    {
        NpgsqlConnection connection = await provider.GetConnectionAsync(default);
        NpgsqlCommand command = connection.CreateCommand();

        command.CommandText = """
                              delete from queues
                              where tgchatid = @tgChatId and tgmessageid = @tgMessageId;
                              """;

        command.Parameters.AddWithValue("tgChatId", tgChatId);
        command.Parameters.AddWithValue("tgMessageId", tgMessageId);

        await command.ExecuteNonQueryAsync(default);
    }
}