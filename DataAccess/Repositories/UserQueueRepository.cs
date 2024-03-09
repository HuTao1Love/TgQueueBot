using Contracts.Repositories;
using Itmo.Dev.Platform.Postgres.Connection;
using Models;
using Npgsql;

namespace DataAccess.Repositories;

public class UserQueueRepository(IPostgresConnectionProvider provider) : IUserQueueRepository
{
    public async IAsyncEnumerable<UserInformation> FindUsersByQueueId(long id)
    {
        NpgsqlConnection connection = await provider.GetConnectionAsync(default);
        NpgsqlCommand command = connection.CreateCommand();

        command.CommandText = """
                              select userid, tgid, name, isadmin, position from users_queues
                              left join users on users_queues.userid = users.id
                              where queueid = @queueId
                              """;

        command.Parameters.AddWithValue("queueId", id);

        await using NpgsqlDataReader reader = await command.ExecuteReaderAsync();

        while (await reader.ReadAsync(default))
        {
            yield return new UserInformation(
                new User(
                    userId: reader.GetInt64(0),
                    tgId: reader.GetInt64(1),
                    name: reader.GetString(2),
                    isAdmin: reader.GetBoolean(3)),
                Position: reader.GetInt32(4));
        }
    }

    public async Task AddUser(long id, User user, int position)
    {
        ArgumentNullException.ThrowIfNull(user);

        NpgsqlConnection connection = await provider.GetConnectionAsync(default);
        NpgsqlCommand command = connection.CreateCommand();

        command.CommandText = """
                              insert into users_queues(userid, queueid, position)
                              values (@userId, @queueId, @position)
                              """;

        command.Parameters.AddWithValue("queueId", id);
        command.Parameters.AddWithValue("userId", user.UserId);
        command.Parameters.AddWithValue("position", position);

        await command.ExecuteNonQueryAsync();
    }

    public async Task RemoveUser(long id, User user)
    {
        ArgumentNullException.ThrowIfNull(user);

        NpgsqlConnection connection = await provider.GetConnectionAsync(default);
        NpgsqlCommand command = connection.CreateCommand();

        command.CommandText = """
                              delete from users_queues
                                  where queueid = @queueId and userid = @userId;
                              """;

        command.Parameters.AddWithValue("queueId", id);
        command.Parameters.AddWithValue("userId", user.UserId);

        await command.ExecuteNonQueryAsync();
    }

    public async Task UpdateUserPosition(long id, User user, int newPosition)
    {
        ArgumentNullException.ThrowIfNull(user);

        NpgsqlConnection connection = await provider.GetConnectionAsync(default);
        NpgsqlCommand command = connection.CreateCommand();

        command.CommandText = """
                              update users_queues
                              set position = @position
                              where queueid = @queueId and userid = @userId;
                              """;

        command.Parameters.AddWithValue("queueId", id);
        command.Parameters.AddWithValue("userId", user.UserId);
        command.Parameters.AddWithValue("position", newPosition);

        await command.ExecuteNonQueryAsync();
    }
}